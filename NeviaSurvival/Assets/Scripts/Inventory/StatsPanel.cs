using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Stats
{
    public class StatsPanel : MonoBehaviour
    {
        public Text HealthIndicator;
        public Text ColdIndicator;
        public Text FoodIndicator;
        public Text EnergyIndicator;
        [SerializeField] TMP_Text DayIndicator;
        [SerializeField] TMP_Text DateIndicator;
        public Text TemperatureIndicator;
        public Text PlayerTemperatureIndicator;


        Player player;
        TOD_Sky time;


        void Start()
        {
            player = FindAnyObjectByType<Player>();
            time = player.DayTime.gameObject.GetComponent<TOD_Sky>();
        }


        void Update()
        {
            HealthIndicator.text = player.Health + " из " + player.maxHealth;
            ColdIndicator.text = player.Cold + " из " + player.maxCold;
            FoodIndicator.text = player.Food + " из " + player.maxFood;
            EnergyIndicator.text = player.Energy + " из " + player.maxEnergy;

            DayIndicator.text = player.DayTime.thisDay.ToString();
            DateIndicator.text = time.Cycle.Day + " " + time.Cycle.Month + " " + time.Cycle.Year;
            TemperatureIndicator.text = Mathf.Round(player.DayTime.temperature) + "C";
            PlayerTemperatureIndicator.text = Mathf.Round(player.feelingTemperature) + "C";
        }
    }
}
