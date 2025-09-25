using Microsoft.AspNetCore.Components;
using System.Timers;

namespace MyCv.UI.Components.Pages
{
    public partial class Home : IDisposable
    {
        private string _terminalText = string.Empty;
        private bool _isTyping = false;
        private Timer? _typingTimer;
        private int _typingIndex = 0;
        private string _fullText = "$ whoami\n> Full Stack Developer & Problem Solver";

        protected override void OnInitialized()
        {
            StartTypingAnimation();
        }

        private void StartTypingAnimation()
        {
            _isTyping = true;
            _terminalText = string.Empty;
            _typingIndex = 0;
            
            _typingTimer?.Dispose();
            _typingTimer = new Timer(50);
            _typingTimer.Elapsed += (sender, e) =>
            {
                InvokeAsync(() =>
                {
                    if (_typingIndex < _fullText.Length)
                    {
                        _terminalText += _fullText[_typingIndex];
                        _typingIndex++;
                        StateHasChanged();
                    }
                    else
                    {
                        _typingTimer?.Stop();
                        _isTyping = false;
                        StateHasChanged();
                    }
                });
            };
            _typingTimer.AutoReset = true;
            _typingTimer.Start();
        }

        public void Dispose()
        {
            _typingTimer?.Dispose();
        }
    }
}
