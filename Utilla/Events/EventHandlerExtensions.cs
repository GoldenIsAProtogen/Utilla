using System;
using Utilla.Tools;

namespace Utilla.Events;

public static class EventHandlerExtensions
{
    public static void SafeInvoke(this EventHandler handler, object sender, EventArgs e)
    {
        if (handler == null) return;

        foreach (Delegate @delegate in handler.GetInvocationList())
        {
            EventHandler callback = (EventHandler)@delegate;
            try
            {
                callback?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }
        }
    }

    public static void SafeInvoke<T>(this EventHandler<T> handler, object sender, T e) where T : EventArgs
    {
        if (handler == null) return;

        foreach (Delegate @delegate in handler.GetInvocationList())
        {
            EventHandler<T> callback = (EventHandler<T>)@delegate;
            try
            {
                callback?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }
        }
    }
}