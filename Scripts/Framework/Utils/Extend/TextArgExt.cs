﻿using Eremite.Model.Effects;

namespace ForwindzCustomPerks.Scripts.Framework.Utils.Extend
{
    public static class TextArgExt
    {
        public static TextArg[] ToTextArgArray(this (TextArgType type, int sourceIndex)[] args)
        {
            TextArg[] array = new TextArg[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                array[i] = new TextArg
                {
                    sourceIndex = args[i].sourceIndex,
                    type = args[i].type
                };
            }

            return array;
        }

        public static TextArg ToTextArg(this (TextArgType type, int sourceIndex) arg)
        {
            return new TextArg
            {
                type = arg.type,
                sourceIndex = arg.sourceIndex
            };
        }
    }
}
