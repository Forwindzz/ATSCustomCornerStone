using Cysharp.Threading.Tasks;
using Eremite;
using Eremite.Services;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Forwindz.Framework.Utils
{
    /// <summary>
    /// This class is intended to provide a better rolling experience.
    /// It has state, but the state will not be saved if the player Save & Load (this is intended design).
    /// 
    /// The design is based on this:
    /// - Bad things will not always happen
    /// - If good things happen a lot, then the player might suffer unfortunate
    /// 
    /// </summary>
    public class StableRNG
    {
        private static float expectChance = 0.0f;
        private static float hitCount = 0;
        /// <summary>
        /// When acculative chance to tolerateGap, the possibility increase
        /// For chance p event, it will absolutely trigger the good result after (1+tolerateGap)/p trails
        /// 
        /// The lucky value is no longer lower than `-tolerateGap-1`
        /// </summary>
        private const float tolerateGap = 1.0f;
        /// <summary>
        /// Measure the feeling of unlucky... no psychology evidence for this model, just a rough estimation.
        /// Rules
        /// - When lucky, dramatically reduce the feeling of unlucky `*factor`
        /// - When unlucky, increase the value, based on unluck times `*unluckyTimes*factor`
        /// - Unlucky times is increased, if there is an interval between unluck events
        /// </summary>
        private static float feelUnlucky = 0.0f;
        private static float unluckyTimes = 0.1f;
        private const float tolerateUnluckyGap = 10.0f; //10 -> around 4 times unlucky
        private const float unluckyBonusRatio = 0.02f;
        private const float luckyReduceFactor = 0.1f;
        private static float lastFeelUnluckyTime = -1.0f;
        private const float regardAsUnluckyInterval = 5.0f;

        public static bool StableRoll(float chance, float weight, bool isPositive)
        {
            if (isPositive)
            {
                return StableRoll(chance, weight);
            }
            else
            {
                return !StableRoll(1.0f - chance, weight);
            }

        }

        public static bool StableRoll(float chance, float weight = 1.0f)
        {
            float bonusChance = Mathf.Max(expectChance - hitCount - tolerateGap, 0.0f) +
                Mathf.Max(unluckyBonusRatio * (feelUnlucky - tolerateUnluckyGap), 0.0f);
            bool result = RNG.Roll(bonusChance + chance);
            if (result)
            {
                hitCount += weight;
                feelUnlucky *= luckyReduceFactor; // reduce the feeling of unlucky a lot
                unluckyTimes *= luckyReduceFactor;
            }
            else
            {
                if (CheckUnluckyInterval())
                {
                    unluckyTimes += weight;
                }
                feelUnlucky += weight * unluckyTimes; // feel unlucky, it negative many times, then feel extremely unlucky 
            }
            expectChance += chance * weight;
            FLog.Info($"RNG Roll: {chance * 100.0f:F2}% + {bonusChance * 100.0f:F2}% | Exp: {(expectChance - hitCount) * 100.0f:F2}% | Unlucky: {feelUnlucky:F3}, Times: {unluckyTimes:F3}");
            return result;
        }

        public static float StableRandom(Vector2 range, float weight = 1.0f, bool higherIsPositive = true)
        {
            float bonusChance = Mathf.Max(expectChance - hitCount - tolerateGap, 0.0f) +
                Mathf.Max(unluckyBonusRatio * (feelUnlucky - tolerateUnluckyGap), 0.0f);
            float rngResult = Mathf.Min(RNG.Float(0.0f, 1.0f) + RNG.Float(0.0f, bonusChance), 1.0f);
            float value = (range.y - range.x) * rngResult + range.x;


            if (higherIsPositive)
            {
                StableRandomAdjustState(rngResult, weight);
            }
            else
            {
                StableRandomAdjustState(1.0f - rngResult, weight);
            }
            FLog.Info($"RNG Range: {rngResult * 100.0f:F2}% + {bonusChance * 100.0f:F2}% | Exp: {(expectChance - hitCount) * 100.0f:F2}% | Unlucky: {feelUnlucky:F3}, Times: {unluckyTimes:F3}");

            return value;
        }

        private static void StableRandomAdjustState(float rngResult, float weight)
        {
            if (rngResult < 0.5f)
            {
                if (CheckUnluckyInterval())
                {
                    unluckyTimes += weight;
                }
                feelUnlucky += (1.0f - rngResult * rngResult) * weight * unluckyTimes;
            }
            else if (rngResult > 0.7f)
            {
                float factor = Mathf.Max(1.01f,1.0f+ luckyReduceFactor) - rngResult;
                factor *= factor;
                feelUnlucky *= factor;
                unluckyTimes *= factor;
            }
            hitCount += rngResult * weight;
            expectChance += 0.5f * weight;
        }

        private static bool CheckUnluckyInterval()
        {
            float seconds = Time.unscaledTime;
            if (lastFeelUnluckyTime < 0.0f)
            {
                FLog.Info($"RNG New Time: {seconds}");
                lastFeelUnluckyTime = seconds;
                return false;
            }
            else
            {
                if (seconds - lastFeelUnluckyTime > regardAsUnluckyInterval)
                {
                    FLog.Info($"RNG Time gap: {seconds - lastFeelUnluckyTime}, New Unlucky Event!");
                    lastFeelUnluckyTime = seconds;
                    return true;
                }
                FLog.Info($"RNG Time gap: {seconds - lastFeelUnluckyTime}, Skip");
                return false;
            }
        }
    }
}
