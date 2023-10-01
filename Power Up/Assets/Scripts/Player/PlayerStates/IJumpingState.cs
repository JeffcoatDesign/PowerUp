using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates {
    public class IJumpingState : IPlayerState
    {
        PlayerController _pc;
        float _stateStartTime;
        float _JumpBuffer = 0.2f;
        float _WallRunBuffer = 0.2f;
        private float _wallCheckDistance = 0.7f;
        private float _minWallrunHeight = 1f;
        private bool WallLeft;
        private bool WallRight;
        public void Enter(PlayerController playerController)
        {
            _pc = playerController;
            _stateStartTime = Time.time;
            _pc.rb.AddForce(Vector3.up * _pc.JumpPower);
        }

        public void Exit()
        {

        }

        public void HandleInput ()
        {
            CheckForWall();
            Vector3 inputVector = new(_pc.MovementInput.x * _pc.PlayerSpeed * _pc.StrafeModifier,
                       0, _pc.MovementInput.y * _pc.PlayerSpeed);
            inputVector = _pc.CameraForward * inputVector;
            _pc.rb.AddForce(inputVector * _pc.SprintSpeed, ForceMode.Force);

            if (_pc.isGrounded && Time.time - _stateStartTime > _JumpBuffer)
                _pc.SetState(new IWalkingState());
            else if ((WallLeft || WallRight) && _pc.MovementInput.y > 0 && AboveGround() && Time.time - _stateStartTime > _WallRunBuffer)
                _pc.SetState(new IWallrunState());
        }
        private void CheckForWall()
        {
            WallLeft = Physics.Raycast(_pc.transform.position, -_pc.transform.right, _wallCheckDistance, _pc.WhatIsWall);
            WallRight = Physics.Raycast(_pc.transform.position, _pc.transform.right, _wallCheckDistance, _pc.WhatIsWall);
        }

        private bool AboveGround()
        {
            return !Physics.Raycast(_pc.transform.position, Vector3.down, _minWallrunHeight, _pc.WhatIsGround);
        }
    }
}