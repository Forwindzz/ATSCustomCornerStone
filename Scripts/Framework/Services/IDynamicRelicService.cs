using Eremite.Buildings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forwindz.Framework.Services
{
    public interface IDynamicRelicService
    {
        public void RemoveExpireEffect(string relicName, int tierIndex, string effectName);
        public void AddExpireEffect(string relicName, int tierIndex, string effectName);
        public void ModifyExpireEffect(string relicName, int tierIndex, string effectName, RelicArrayOperation op);
        public void RemoveModifyExpireEffect(string relicName, int tierIndex, string effectName, RelicArrayOperation op);
        public void AddProcessSpeed(string relicName, float speed);
        public void SetProcessSpeed(string relicName, float speed);
        public float GetProcessSpeed(string relicName);

        public IObservable<Relic> OnRelicExpire { get; }
    }
}
