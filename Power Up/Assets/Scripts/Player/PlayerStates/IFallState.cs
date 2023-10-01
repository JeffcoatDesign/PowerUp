using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates {
    public class IFallState : IPlayerState
    {
        PlayerController _pc;
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
            if (_pc.isGrounded)
                _pc.SetState(new IWalkingState());
            else
            {
                Vector3 inputVector = new(_pc.MovementInput.x * _pc.PlayerSpeed * _pc.StrafeModifier,
                    0, _pc.MovementInput.y * _pc.PlayerSpeed);
                inputVector = _pc.CameraForward * inputVector;
                _pc.rb.AddForce(inputVector * _pc.SprintSpeed, ForceMode.Force);
            }
        }
    }
}