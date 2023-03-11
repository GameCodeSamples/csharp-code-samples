using Cysharp.Threading.Tasks;

namespace Core.Runtime.Interactables
{
    public class TestCondition : Condition
    {
        public override async UniTask<bool> Evaluate()
        {
            return await HasUserEnoughMoney();
        }

        private async UniTask<bool> HasUserEnoughMoney()
        {
            await UniTask.Delay(100);
            return true;
        }
    }
}