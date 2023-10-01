using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    public class IStandingState : IPlayerState
    {
        private PlayerController _pc;
        public void Enter (PlayerController playerController)
        {
            _pc = playerController;
            _pc.ToggleSprint(false);
        }

        public void Exit ()
        {

        }

        public void HandleInput()
        {
            if (_pc == null)
                return;
            if (_pc.jumpPressed)
                _pc.SetState(new IJumpingState());
            else if (_pc.isCrouching)
                _pc.SetState(new ICrouchState());
            else if (_pc.MovementInput.magnitude != 0)
                _pc.SetState(new IWalkingState());
        }
    }
}