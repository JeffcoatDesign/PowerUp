using UnityEngine;

namespace PlayerStates
{
    public interface IPlayerState
    {
        public void Enter(PlayerController playerController);
        public void Exit();
        public void HandleInput();
    }
}