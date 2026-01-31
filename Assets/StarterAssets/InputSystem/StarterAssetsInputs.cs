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
		public bool ability;
		

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
			if(cursorInputForLook)
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

		public void OnColor1(InputValue value)
		{
			Color1Input(value.isPressed);
		}

        public void OnColor2(InputValue value)
        {
            Color2Input(value.isPressed);
        }

        public void OnColor3(InputValue value)
        {
            Color3Input(value.isPressed);
        }

		public void OnAbility(InputValue value)
		{
			AbilityInput(value.isPressed);
		}
#endif


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

		public void Color1Input (bool newColorState)
		{
			if (newColorState)
			{
				GameManager.Singleton.currentColor = GameManager.colors.RED;
                GameManager.Singleton.ColorChangeEvent.Invoke();
            }
		}

        public void Color2Input(bool newColorState)
        {
            if (newColorState)
            {
                GameManager.Singleton.currentColor = GameManager.colors.GREEN;
                GameManager.Singleton.ColorChangeEvent.Invoke();
            }
        }

        public void Color3Input(bool newColorState)
        {
            if (newColorState)
            {
                GameManager.Singleton.currentColor = GameManager.colors.BLUE;
                GameManager.Singleton.ColorChangeEvent.Invoke();
            }
        }

		public void AbilityInput(bool newAbilityState)
		{
			ability = newAbilityState;
		}

        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}