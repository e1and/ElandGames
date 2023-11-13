using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
        public Text StaminaIndicator;
        public Text OxygenIndicator;
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
            StaminaIndicator.text = player.Stamina + " из " + player.maxStamina;
            OxygenIndicator.text = player.Oxygen + " из " + player.maxOxygen;

            DayIndicator.text = player.DayTime.thisDay.ToString();
            DateIndicator.text = time.Cycle.Day + " " + Month(time.Cycle.Month) + " " + time.Cycle.Year;
            TemperatureIndicator.text = Mathf.Round(player.DayTime.temperature) + "°C";
            PlayerTemperatureIndicator.text = Mathf.Round(player.feelingTemperature) + "°C";
        }

        string Month(int i)
        {
            switch (i)
            {
                case 1: return "января";
                case 2: return "февраля";
                case 3: return "марта";
                case 4: return "апреля";
                case 5: return "мая";
                case 6: return "июня";
                case 7: return "июля";
                case 8: return "августа";
                case 9: return "сентября";
                case 10: return "октября";
                case 11: return "ноября";
                case 12: return "декабря";
                default: return "января";
            }

        }
    }
}
