using UnityEngine;
using System.Collections.Generic;

namespace SG
{
    public class PlayerCombatTracker : MonoBehaviour
    {
        public static PlayerCombatTracker Instance; // Singleton for easy access

        private List<float> attackTimestamps = new List<float>(); // Stores attack times
        private Dictionary<string, int> attackTypeCount = new Dictionary<string, int>(); // Tracks attack patterns

        public float aggressionThreshold = 3f; // Time window for "aggressive" player attacks
   

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void RegisterPlayerHit()
        {
            float currentTime = Time.time;
            attackTimestamps.Add(currentTime);

            // Remove old attacks beyond the threshold time
            attackTimestamps.RemoveAll(time => currentTime - time > aggressionThreshold);

            // Notify the boss to adjust dodge behavior
            BossDodgeHandler.Instance?.UpdateDodgeWeight(CalculateDodgeWeight());
        }

        private float CalculateDodgeWeight()
        {
            float dodgeWeight = 0.2f; // Base dodge weight

            int attackCount = attackTimestamps.Count;

            // Increase weight if the player attacks frequently
            if (attackCount >= 3)
            {
                dodgeWeight += 0.3f; // Base increase for aggressive behavior
                dodgeWeight += (attackCount - 3) * 0.1f; // Extra 0.1 per additional attack
            }

            return Mathf.Clamp(dodgeWeight, 0.2f, 0.9f); // Prevent extreme values
        }
    }
}