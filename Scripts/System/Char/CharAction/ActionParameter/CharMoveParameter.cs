using UnityEngine;

namespace Client
{
    public class CharMoveParameter : CharActionParameter
    {
        public Vector3 Destination;

        public CharMoveParameter(Vector3 destination)
        {
            Destination = destination;
        }

        public CharMoveParameter(CharBase target)
        {
            if(target == null)
            {
                Debug.LogError("없는데다가 움직이라고 하면 곤란함.");
            }
            Destination = target.transform.position;
        }

        public CharMoveParameter(long targetCharUID)
        {
            CharBase target = CharManager.Instance.GetFieldChar(targetCharUID);
            if(target == null) Debug.LogError("없는데다가 움직이라고 하면 곤란함.");
            Destination = target.transform.position;
        }
    }
}