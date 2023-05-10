using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAgent : Agent
{
	//[HideInInspector]
	[SerializeField] public Perception perception;

	public PathFollower pathFollower;
	public StateMachine stateMachine = new StateMachine();

	public BoolRef enemySeen;
	public FloatRef enemyDistance;
	public FloatRef health;
	public FloatRef timer;

	public BoolRef atDestination;

	public GameObject enemy { get; set; }

	private void Start()
	{
		stateMachine.AddState(new IdleState(this, typeof(IdleState).Name));
		stateMachine.AddState(new PatrolState(this, typeof(PatrolState).Name));
		stateMachine.AddState(new ChaseState(this, typeof(ChaseState).Name));
		stateMachine.AddState(new DeathState(this, typeof(DeathState).Name));
		stateMachine.AddState(new AttackState(this, typeof(AttackState).Name));
		stateMachine.AddState(new EvadeState(this, typeof(EvadeState).Name));
		stateMachine.AddState(new RoamState(this, typeof(RoamState).Name));


		stateMachine.AddTransition(typeof(IdleState).Name, new Transition(new Condition[] { new BoolCondition(enemySeen, true), new FloatCondition(health, Condition.Predicate.GREATER, 30) }), typeof(ChaseState).Name); 
		//stateMachine.AddTransition(typeof(IdleState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.GREATER,30) }), typeof(ChaseState).Name); 
		stateMachine.AddTransition(typeof(IdleState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESSEQUAL, 30) }), typeof(EvadeState).Name); 
		stateMachine.AddTransition(typeof(IdleState).Name, new Transition(new Condition[] { new FloatCondition(timer, Condition.Predicate.LESS, 0) }), typeof(PatrolState).Name); 
		stateMachine.AddTransition(typeof(IdleState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESSEQUAL, 0) }), typeof(DeathState).Name); 

		stateMachine.AddTransition(typeof(ChaseState).Name, new Transition(new Condition[] { new BoolCondition(enemySeen, false) }), typeof(IdleState).Name); 
		stateMachine.AddTransition(typeof(ChaseState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESSEQUAL, 0) }), typeof(DeathState).Name); 
		stateMachine.AddTransition(typeof(ChaseState).Name, new Transition(new Condition[] { new FloatCondition(enemyDistance, Condition.Predicate.LESSEQUAL, 2) }), typeof(AttackState).Name); 
		stateMachine.AddTransition(typeof(ChaseState).Name, new Transition(new Condition[] { new BoolCondition(enemySeen, true), new FloatCondition(health, Condition.Predicate.LESSEQUAL, 30) }), typeof(EvadeState).Name); 

		stateMachine.AddTransition(typeof(PatrolState).Name, new Transition(new Condition[] { new BoolCondition(enemySeen, true) }), typeof(ChaseState).Name); 
		stateMachine.AddTransition(typeof(PatrolState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESSEQUAL, 0) }), typeof(DeathState).Name); 
		stateMachine.AddTransition(typeof(PatrolState).Name, new Transition(new Condition[] { new FloatCondition(timer, Condition.Predicate.LESSEQUAL, 0) }), typeof(RoamState).Name);

		stateMachine.AddTransition(typeof(AttackState).Name, new Transition(new Condition[] { new FloatCondition(timer, Condition.Predicate.LESSEQUAL, 0) }), typeof(ChaseState).Name); 
		stateMachine.AddTransition(typeof(AttackState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESSEQUAL, 30) }), typeof(EvadeState).Name); 

		stateMachine.AddTransition(typeof(EvadeState).Name, new Transition(new Condition[] { new BoolCondition(enemySeen,false) }), typeof(IdleState).Name);
		stateMachine.AddTransition(typeof(EvadeState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESSEQUAL, 0) }), typeof(DeathState).Name);
		stateMachine.AddTransition(typeof(EvadeState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESSEQUAL, 0) }), typeof(DeathState).Name);

		stateMachine.AddTransition(typeof(RoamState).Name, new Transition(new Condition[] { new BoolCondition(atDestination, true) }), typeof(IdleState).Name);


		stateMachine.SetState(stateMachine.StateFromName(typeof(IdleState).Name));
	}

	private void Update()
	{
		//update params
		var enemies = perception.GameObjects();
		enemySeen.value = (enemies.Length != 0);
		enemy = (enemies.Length != 0) ? enemies[0] : null;
		enemyDistance.value = (enemy != null) ? (Vector3.Distance(transform.position, enemy.transform.position)) : float.MaxValue;


		timer.value -= Time.deltaTime;

		stateMachine.Update();

		animator.SetFloat("speed", movement.velocity.magnitude); 
		
	}

	private void OnGUI()
	{
		Vector2 screen = Camera.main.WorldToScreenPoint(transform.position);

		GUI.Label(new Rect(screen.x, Screen.height - screen.y, 300, 20), stateMachine.GetStateName());

	}

}
