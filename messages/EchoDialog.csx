using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

// For more information about this template visit http://aka.ms/azurebots-csharp-basic
[Serializable]
public class EchoDialog : IDialog<object>
{
    protected int count = 1;
    protected Random random = new Random();

    public Task StartAsync(IDialogContext context)
    {
        try
        {
            context.Wait(MeaningfulMessageReceivedAsync);
        }
        catch (OperationCanceledException error)
        {
            return Task.FromCanceled(error.CancellationToken);
        }
        catch (Exception error)
        {
            return Task.FromException(error);
        }

        return Task.CompletedTask;
    }
    
    public virtual async Task MeaningfulMessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
        var message = await argument;
        var rnd = DateTime.Now.Milliseconds.ToString();
        var doSwitch = rnd.EndsWith("0");
        if (message.Text.EndsWith("?"))
        {
            if (doSwitch)
                await context.PostAsync($"В смысле {message.Text}");
            else
                await context.PostAsync($"Не понял");
            
        }
        else
        {
            if (doSwitch)
                await context.PostAsync($"Не понял");
            else
                await context.PostAsync($"В смысле {message.Text}?");
            
                
        }
        context.Wait(MeaningfulMessageReceivedAsync);
    }
    
    public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
        var message = await argument;
        if (message.Text == "reset")
        {
            PromptDialog.Confirm(
                context,
                AfterResetAsync,
                "Are you sure you want to reset the count?",
                "Didn't get that!",
                promptStyle: PromptStyle.Auto);
        }
        else
        {
            await context.PostAsync($"{this.count++}: You said {message.Text}?");
            context.Wait(MessageReceivedAsync); 
        }
    }

    public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
    {
        var confirm = await argument;
        if (confirm)
        {
            this.count = 1;
            await context.PostAsync("Reset count.");
        }
        else
        {
            await context.PostAsync("Did not reset count.");
        }
        context.Wait(MessageReceivedAsync);
    }
}