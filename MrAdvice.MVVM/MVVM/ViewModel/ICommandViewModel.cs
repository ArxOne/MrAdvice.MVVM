using System.Reflection;

namespace ArxOne.MrAdvice.MVVM.ViewModel;

public interface ICommandViewModel
{
    object InvokeCommand(MethodInfo commandMethod, object[] parameters);
}
