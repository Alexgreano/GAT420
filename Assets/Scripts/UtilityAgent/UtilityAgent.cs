using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UtilityAgent : Agent
{
    [SerializeField] Perception perception;
    [SerializeField] MeterUI meter;

    static readonly float MIN_SCORE = 0.2f;

    Need[] needs;
    UtilityObject activeUtilityObject;
    public bool isUsingUtilityObject { get { return activeUtilityObject != null; } }

    public float happiness
    {
        get
        {
            float totalMotive = 0;
            foreach (var need in needs)
            {
                totalMotive += need.motive;
            }
            return 1 - totalMotive / needs.Length;
        }
    }

    void Start()
    {
        needs = GetComponentsInChildren<Need>();

        meter.text.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        //animator.SetFloat("speed", movement.velocity.magnitude);

        if(activeUtilityObject == null)
        {
            var gameObjects = perception.GameObjects();
            List<UtilityObject> utilityObjects = new List<UtilityObject>();
            foreach (var go in gameObjects)
            {
                if (go.TryGetComponent<UtilityObject>(out UtilityObject utilityObject)){
                    utilityObject.visible = true;
                    utilityObject.score = GetUtilityObjectScore(utilityObject);
                    if (utilityObject.score > 0.2f) utilityObjects.Add(utilityObject);
                }
            }

            bool allCoolDown = true;
            foreach(UtilityObject uo in utilityObjects)
            {
                if(uo.cooldown <= 0)
                {
                    allCoolDown = false;
                    break;
                }
            }
            if (!allCoolDown)
            {
                do
                {
                    activeUtilityObject = (utilityObjects.Count() > 0) ? (
                        (activeUtilityObject == null) ? GetHighestUtilityObject(utilityObjects.ToArray()) : GetRandomUtilityObject(utilityObjects.ToArray())
                        ) : null;

                } while (activeUtilityObject != null && activeUtilityObject.cooldown > 0);
            }

            
            if (activeUtilityObject != null)
            {
                StartCoroutine(ExecuteUtilityObject(activeUtilityObject));
            }


        }
    }

    private void LateUpdate()
    {
        meter.slider.value = happiness;
        meter.worldPosition = transform.position + Vector3.up * 4;
        
    }

    IEnumerator ExecuteUtilityObject(UtilityObject utilityObject)
    {
               movement.MoveTowards(utilityObject.location.position);
            while (Vector3.Distance(transform.position, utilityObject.location.position) > 0.25f)
            {
                print(Vector3.Distance(transform.position, utilityObject.location.position));
                Debug.DrawLine(transform.position, utilityObject.location.position, Color.red);

                yield return null;
            }

        print("start");
        if (utilityObject.Effect != null) utilityObject.Effect.SetActive(true);


        yield return new WaitForSeconds(utilityObject.duration);

        print("stop");
        if (utilityObject.Effect != null) utilityObject.Effect.SetActive(false);
        utilityObject.cooldown = 10;

        ApplyUtilityObject(utilityObject);

        activeUtilityObject = null;

        yield return null;
    }

    void ApplyUtilityObject(UtilityObject utilityObject)
    {
        foreach (var effector in utilityObject.effectors)
        {
            Need need = GetNeedByType(effector.type);
            if (need != null)
            {
                need.input += effector.change;
                need.input = Mathf.Clamp(need.input, -1, 1);
            }

        }
    }

    float GetUtilityObjectScore(UtilityObject utilityObject)
    {
        float score = 0;

        foreach (var effector in utilityObject.effectors)
        {
            Need need = GetNeedByType(effector.type);
            if (need != null)
            {
                float futureNeed = need.getMotive(need.input + effector.change);
                score += need.motive - futureNeed;
            }
        }

        return score;
    }

    Need GetNeedByType(Need.Type type)
    {
        return needs.First(need => need.type == type);
    }

    UtilityObject GetHighestUtilityObject(UtilityObject[] utilityObjects)
    {
        UtilityObject highestUtilityObject = null;
        float highestScore = MIN_SCORE;
        foreach (var utilityObject in utilityObjects)
        {

            // get the score of the utility object
            // if score > highest score then set new highest score and highest utility object
            float score = utilityObject.score;
            if(score > highestScore)
            {
                highestUtilityObject = utilityObject;
            }
        }
        return highestUtilityObject;
    }

    UtilityObject GetRandomUtilityObject(UtilityObject[] utilityObjects)
    {
        // evaluate all utility objects
        float[] scores = new float[utilityObjects.Length];
        float totalScore = 0;
        for (int i = 0; i < utilityObjects.Length; i++)
        {
            // <get the score of the utility objects[i]>
            // <set the scores[i] to the score>
            // <add score to total score>
            scores[i] = GetUtilityObjectScore(utilityObjects[i]);
            totalScore += scores[i];
        }

        // select random utility object based on score
        // the higher the score the greater the chance of being randomly selected
 
        // <float random = value between 0 and totalScore>
        float random = Random.Range(0, totalScore);
        for (int i = 0; i < scores.Length; i++)
        {
            
            // <check if random value is less than scores[i]>
            // <return utilityObjects[i] if less than>
            // <subtract scores[i] from random value>
            if (random < scores[i]) return utilityObjects[i];
            random -= scores[i];
        }
        return null;
    }
}

