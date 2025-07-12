using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool throwInput;
		public bool pickupInput;
		public bool attack;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if (cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnThrow(InputValue value)
		{
			// This method can be used to handle any throw input if needed
			// Currently, it does nothing
			// Debug.Log("Throw input received, but no action defined.");
			ThrowInput(value.isPressed);
		}

		public void OnPickup(InputValue value)
		{
			// This method can be used to handle any pickup input if needed
			// Currently, it does nothing
			// Debug.Log("Pickup input received, but no action defined.");
			PickupInput(value.isPressed);
		}

		public void OnAttack(InputValue value)
		{
			// This method can be used to handle any attack input if needed
			// Currently, it does nothing
			// Debug.Log("Attack input received, but no action defined.");
			AttackInput(value.isPressed);
		}
#endif

		public void AttackInput(bool newAttackState)
		{
			attack = newAttackState;
		}
		public void ThrowInput(bool newThrowState)
		{
			throwInput = newThrowState;
		}

		public void PickupInput(bool newPickupState)
		{
			pickupInput = newPickupState;
		}

		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			// Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}

}