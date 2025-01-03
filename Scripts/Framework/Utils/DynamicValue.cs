﻿using Eremite.Services.World;
using Forwindz.Framework.Utils.Extend;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Forwindz.Framework.Utils
{

    public interface IDynamicValue<T>
    {
        public T BaseValue { get; }
        public T CurrentValue { get; set; }
        public void RestoreOriginalValue();
    }

    public class DynamicValueFloat : IDynamicValue<float>
    {
        private readonly float baseValue;
        private float cacheValue;
        // the real value, it might be changed by other mods
        private readonly Func<float> getter;
        // += operation
        private readonly Action<float> adder;

        // original value
        public float BaseValue => baseValue;
        // in my mod the current value is ...
        public float CurrentValue { get => cacheValue; set => SetNewValue(value); }
        // this is the actual value, are impacted by all mods.
        public float RealValue => getter();

        public DynamicValueFloat(Func<float> getter, Action<float> adder)
        {
            this.getter = getter;
            this.adder = adder;
            cacheValue = baseValue = this.getter();
        }

        public void SetNewValue(float newValue)
        {
            adder(newValue - cacheValue);
            cacheValue = newValue;
        }

        public void RestoreOriginalValue()
        {
            adder(baseValue - getter());
            cacheValue = baseValue;
        }
    }

    public class DynamicValueInt : IDynamicValue<int>
    {
        private readonly int baseValue;
        private int cacheValue;
        // the real value, it might be changed by other mods
        private readonly Func<int> getter;
        // += operation
        private readonly Action<int> adder;

        // original value
        public int BaseValue => baseValue;
        // in my mod the current value is ...
        public int CurrentValue { get => cacheValue; set => SetNewValue(value); }
        // this is the actual value, are impacted by all mods.
        public int RealValue => getter();

        public DynamicValueInt(Func<int> getter, Action<int> adder)
        {
            this.getter = getter;
            this.adder = adder;
            cacheValue = baseValue = this.getter();
        }

        public void SetNewValue(int newValue)
        {
            adder(newValue - cacheValue);
            cacheValue = newValue;
        }

        public void RestoreOriginalValue()
        {
            adder(baseValue - getter());
            cacheValue = baseValue;
        }
    }

    public class DynamicValueArray<T> : IDynamicValue<T[]>
    {
        private readonly T[] baseValue;
        // the real value, it might be changed by other mods
        private readonly Func<T[]> getter;
        // += operation
        private readonly Action<T[]> setter;

        // original value
        public T[] BaseValue => baseValue;
        // in my mod the current value is ...
        public T[] CurrentValue { get => this.getter(); set => setter(value); }
        // this is the actual value, are impacted by all mods.
        public T[] RealValue => getter();

        public DynamicValueArray(Func<T[]> getter, Action<T[]> setter)
        {
            this.getter = getter;
            this.setter = setter;
            baseValue = this.getter();
        }

        public void AddNewValue(T newValue)
        {
            setter(getter().ForceAdd(newValue));
        }

        public void Remove(int index)
        {
            setter(getter().ForceRemove<T>(index));
        }

        public void RestoreOriginalValue()
        {
            setter(baseValue);
        }
    }


}
