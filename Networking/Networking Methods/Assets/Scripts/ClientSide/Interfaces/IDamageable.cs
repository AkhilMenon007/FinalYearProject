using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FYP.Client 
{
    public interface IDamageable
    {
        void TakeDamage(DamageData damageData);
    }
    public struct DamageData
    {
        public int damageAmount;
    }
}
