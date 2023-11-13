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
            HealthIndicator.text = player.Health + " �� " + player.maxHealth;
            ColdIndicator.text = player.Cold + " �� " + player.maxCold;
            FoodIndicator.text = player.Food + " �� " + player.maxFood;
            EnergyIndicator.text = player.Energy + " �� " + player.maxEnergy;
            StaminaIndicator.text = player.Stamina + " �� " + player.maxStamina;
            OxygenIndicator.text = player.Oxygen + " �� " + player.maxOxygen;

            DayIndicator.text = player.DayTime.thisDay.ToString();
            DateIndicator.text = time.Cycle.Day + " " + Month(time.Cycle.Month) + " " + time.Cycle.Year;
            TemperatureIndicator.text = Mathf.Round(player.DayTime.temperature) + "�C";
            PlayerTemperatureIndicator.text = Mathf.Round(player.feelingTemperature) + "�C";
        }

        string Month(int i)
        {
            switch (i)
            {
                case 1: return "������";
                case 2: return "�������";
                case 3: return "�����";
                case 4: return "������";
                case 5: return "���";
                case 6: return "����";
                case 7: return "����";
                case 8: return "�������";
                case 9: return "��������";
                case 10: return "�������";
                case 11: return "������";
                case 12: return "�������";
                default: return "������";
            }

        }
    }
}
