using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AcmHouse.Enums
{
    public enum AccommodationType
    {
        None, Апартаменты, Дом, Вилла, Коттедж, Офис, Склад
    }

    /// <summary>Result of a sign-in attempt.</summary>
    public enum LoginResult
    {
        Failed = 0,
        Employee = 1,
        Administrator = 2
    }
}
