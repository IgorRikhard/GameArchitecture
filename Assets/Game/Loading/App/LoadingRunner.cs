using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Loading
{
    public class LoadingRunner
    {
        public async UniTask Run(List<ILoadingOperation> operations, LoadingScreen loadingScreen,
            CancellationToken cancellationToken = default)
        {
            if (operations == null || operations.Count == 0)
            {
                return;
            }

            float completed = 0f;
            for (int i = 0; i < operations.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ILoadingOperation op = operations[i];
                if (loadingScreen != null)
                {
                    loadingScreen.SetTip(op.Description);
                    loadingScreen.SetProgress(completed / operations.Count);
                }
                
                await op.Load(cancellationToken);

                completed += 1f;
                if (loadingScreen != null)
                {
                    loadingScreen.SetProgress(completed / operations.Count);
                }
                Debug.Log("completed / operations.Count: " + (completed / operations.Count));

            }
        }
    }
}