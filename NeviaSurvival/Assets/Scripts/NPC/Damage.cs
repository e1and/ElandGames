using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public int damage = 35;

    public int SingleDamage()
    {
        if (damageCoolDownTimer <= 0)
        {
            damageCoolDownTimer = 0.5f;
            return damage;
        }
        else
        return 0;
    }

    // ������� ��� ����, ����� �� ��������� ��������� ������ �� ���� ���
    public float damageCoolDownTimer;
    private void Update()
    {
        if (damageCoolDownTimer > 0)
        {
            damageCoolDownTimer -= Time.deltaTime;
        }
    }
}
