using System;
using System.Collections.Generic;
using System.Text;

namespace FYPServer
{
    interface IDamageable
    {
        void TakeDamage(ushort damageAmount);
    }
}
