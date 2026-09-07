using UnityEngine.Playables;
using UnityEngine;
namespace Client
{
    public class SkillMarkerReceiver : MonoBehaviour, INotificationReceiver
    {
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is SkillTimeLineMarker skillMarker)
            {
                SkillBase provider = GetComponent<SkillBase>();
                
                skillMarker.InitInput(provider.InputParameter, provider.Director);
                skillMarker.MarkerAction();

            }
        }
    }
}


