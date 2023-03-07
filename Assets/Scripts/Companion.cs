using UnityEngine;
using System.Collections;

namespace Completed
{
	public class Companion : MovingObject
	{	
		private Animator animator;							
		private Transform target;						
		
		private bool skipMove;
		
		protected override void Start ()
		{
			
			GameManager.instance.AddCompanion (this);
			
			animator = GetComponent<Animator> ();
			
			target = GameObject.FindGameObjectWithTag ("Player").transform;
			
			base.Start ();
		}
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
			
			// if(skipMove)
			// {
			// 	skipMove = false;
			// 	return;
			// }
			
			base.AttemptMove <T> (xDir, yDir);
			
			skipMove = true;
		}		
		
		public void MoveCompanion ()
		{
			int xDir = 0;
			int yDir = 0;
			
			if(Mathf.Abs (target.position.x -1 - transform.position.x) < float.Epsilon)
				
				yDir = target.position.y -1 > transform.position.y ? 1 : -1;
			
			
			else
				xDir = target.position.x -1 > transform.position.x ? 1 : -1;
			
			AttemptMove <Player> (xDir, yDir);
		}
		
		
		protected override void OnCantMove <T> (T component)
		{
			
		}
	}
}
