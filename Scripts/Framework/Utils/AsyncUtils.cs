using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Forwindz.Framework.Utils
{
    public static class AsyncUtils
    {
        public static async Task WaitForConditionAsync(Func<bool> condition, int checkInterval = 100, CancellationToken cancellationToken = default)
        {
            while (!condition())
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new TaskCanceledException();

                await Task.Delay(checkInterval, cancellationToken);
            }
        }
    }
}
