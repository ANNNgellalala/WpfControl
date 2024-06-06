using System.Windows.Input;

namespace Calendar;

public class Command : ICommand
{
    private readonly Action _action;
    private readonly Func<bool>? _condition;
    
    public bool CanExecute(
        object? parameter) =>
        _condition?.Invoke() ?? false;

    public void Execute(
        object? parameter) =>
        _action();

    public event EventHandler? CanExecuteChanged;
    
    public Command(
        Action action,
        Func<bool> condition)
    {
        _action = action;
        _condition = condition;
    }
}

public class Command<T> : ICommand
{
    private readonly Action<T> _action;

    private readonly Func<T, bool> _condition;

    public bool CanExecute(
        object? parameter)
    {
        return parameter is not null && _condition((T)parameter);
    }

    public void Execute(
        object? parameter) =>
        _action((T)parameter!);

    public event EventHandler? CanExecuteChanged;
    
    public Command(
        Action<T> action,
        Func<T, bool> condition)
    {
        _action = action;
        _condition = condition;
    }
}