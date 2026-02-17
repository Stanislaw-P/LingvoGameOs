using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using NuGet.Protocol.Plugins;

namespace LingvoGameOs.Helpers
{
    public class EmailService
    {
        private readonly MailSettings mailSettings;
        private readonly ILogger<EmailService> _logger;
        private readonly string adminEmail = "rudzyng@yandex.ru";

        public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
        {
            this.mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task<bool> TrySendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
                emailMessage.To.Add(new MailboxAddress(mailSettings.DisplayName, email));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = message
                };
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(mailSettings.Host, mailSettings.Port);
                    await client.AuthenticateAsync(mailSettings.Mail, mailSettings.Password);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
                return true;
            }
            catch (SmtpCommandException ex)
            {
                _logger.LogError(ex, "При попытке отправить письмо возникло исключение: {@SendEmailData}", new
                {
                    DeveloperEmail = email,
                    TextMessage = message,
                    RequestTime = DateTimeOffset.UtcNow,
                    ResponseStatusCode = 500
                });
                return false;
            }
        }

        public async Task<bool> TrySendModerationRejectionAsync(string devName, string devEmail, string gameName, string feedbackText)
        {
            string subject = "Игра не прошла модерацию";
            string htmlMessage = BuildModerationEmailHtml(devName, gameName, feedbackText);
            return await TrySendEmailAsync(devEmail, subject, htmlMessage);
        }

        public async Task<bool> TrySendAboutPublicationAsync(string devName, string devEmail, string gameName)
        {
            string subject = "Игра опубликована";
            string htmlMessage = BuildPublicationEmailHtml(devName, gameName);
            return await TrySendEmailAsync(devEmail, subject, htmlMessage);
        }

        public async Task<bool> TrySendRefusalGameAsync(string devName, string devEmail, string gameName)
        {
            string subject = "Игра удалена";
            string htmlMessage = BuildRefusalGameEmailHtml(devName, gameName);
            return await TrySendEmailAsync(devEmail, subject, htmlMessage);
        }

        public async Task<bool> TrySendGameUpdateByDevAsync(string devName, string devEmail, string gameName, int gameId)
        {
            string subject = "Игра обновлена и ожидает подтверждения";
            string htmlMessage = BuildGameUpdateNotificationEmailHtml(devName, devEmail, gameName, gameId);
            return await TrySendEmailAsync(adminEmail, subject, htmlMessage);
        }

        public async Task<bool> TrySendAboutDeactivateGameAsync(string devName, string devEmail, string gameName)
        {
            string subject = "Игра деактивирована";
            string htmlMessage = BuildDeactivationEmailHtml(devName, gameName);
            return await TrySendEmailAsync(devEmail, subject, htmlMessage);
        }


        private string BuildModerationEmailHtml(string developerName, string gameName, string feedbackText)
        {
            return $@"
<!DOCTYPE html>
<html lang='ru'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        * {{
            padding: 0;
            margin: 0;
            border: 0;
            box-sizing: border-box;
        }}
        
        body {{
            background: #141C30;
            color: #F8FAFE;
            font-family: Arial, sans-serif;
            line-height: 1.6;
            margin: 0;
            padding: 0;
        }}
        
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 40px 20px;
        }}
        
        .email-header {{
            text-align: center;
            margin-bottom: 40px;
            padding-bottom: 20px;
            border-bottom: 2px solid #43495C;
        }}
        
        .email-logo {{
            font-family: Arial, sans-serif;
            font-size: 32px;
            color: #3F69D2;
            margin-bottom: 20px;
            font-weight: bold;
        }}
        
        .email-title {{
            font-size: 28px;
            font-weight: 600;
            color: #D54646;
            margin-bottom: 10px;
        }}
        
        .email-subtitle {{
            font-size: 18px;
            color: #666D80;
        }}
        
        .email-content {{
            background: #F8FAFE;
            border-radius: 10px 70px;
            padding: 40px;
            margin-bottom: 30px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
        }}
        
        .email-section {{
            margin-bottom: 30px;
        }}
        
        .email-section:last-child {{
            margin-bottom: 0;
        }}
        
        .email-greeting {{
            font-size: 16px;
            line-height: 1.6;
            color: #182032;
            margin-bottom: 20px;
        }}
        
        .email-feedback {{
            background: #FFFFFF;
            padding: 25px;
            border-radius: 10px 50px;
            margin: 25px 0;
            border-left: 4px solid #3F69D2;
        }}
        
        .email-feedback-title {{
            font-size: 18px;
            font-weight: 600;
            color: #182032;
            margin-bottom: 15px;
        }}
        
        .email-feedback-text {{
            font-size: 15px;
            line-height: 1.6;
            color: #182032;
            white-space: pre-line;
        }}
        
        .email-note {{
            background: #FFFFFF;
            border-left: 4px solid #5B7FD9;
            padding: 20px;
            margin: 25px 0;
            border-radius: 0 10px 10px 0;
            color: #182032;
        }}
        
        .email-footer {{
            text-align: center;
            padding-top: 30px;
            border-top: 1px solid #43495C;
            color: #666D80;
        }}
        
        .email-contact {{
            margin: 20px 0;
            padding: 15px;
            background: #F8FAFE;
            border-radius: 10px;
        }}
        
        .email-support {{
            font-size: 14px;
            margin-top: 15px;
        }}
        
        .email-signature {{
            margin-top: 30px;
            color: #666D80;
        }}
        
        a {{
            color: #3F69D2;
            text-decoration: none;
        }}
        
        strong {{
            font-weight: 600;
        }}
        
        h3 {{
            font-size: 20px;
            font-weight: 600;
            color: #182032;
            margin-bottom: 20px;
        }}
        
        @media (max-width: 768px) {{
            .email-container {{
                padding: 20px 15px;
            }}
            
            .email-content {{
                padding: 30px 20px;
                border-radius: 10px 50px;
            }}
            
            .email-title {{
                font-size: 24px;
            }}
        }}
        
        @media (max-width: 480px) {{
            .email-content {{
                padding: 20px 15px;
            }}
            
            .email-feedback {{
                padding: 20px;
            }}
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='email-header'>
            <div class='email-logo'>Рудзынг</div>
            <h1 class='email-title'>Игра не прошла модерацию</h1>
            <p class='email-subtitle'>Уведомление от команды модерации</p>
        </div>

        <div class='email-content'>
            <div class='email-section'>
                <p class='email-greeting'>
                    Уважаемый(ая) <strong>{developerName}</strong>,
                </p>
                <p class='email-greeting'>
                    Команда «Рудзынг» благодарит вас за интерес к нашей платформе и за предоставленную игру 
                    <strong>«{gameName}»</strong> для проверки.
                </p>
            </div>

            <div class='email-section'>
                <p class='email-greeting'>
                    Мы внимательно изучили ваш проект и, к сожалению, не можем одобрить его публикацию в текущем виде.
                </p>
            </div>

            <div class='email-section'>
                <div class='email-feedback'>
                    <div class='email-feedback-title'>Замечания по модерации:</div>
                    <div class='email-feedback-text'>{feedbackText}</div>
                </div>
            </div>

            <div class='email-section'>
                <h3>Что делать дальше?</h3>
                <p class='email-greeting'>
                    После устранения указанных замечаний, пожалуйста, повторно загрузите новую сборку игры в ваш кабинет разработчика. 
                    Мы проведем повторную, ускоренную модерацию.
                </p>
                
                <div class='email-note'>
                    <p>
                        Если у вас возникли вопросы по какому-либо из пунктов, не стесняйтесь отвечать на это письмо. 
                        Мы готовы дать более развернутые комментарии.
                    </p>
                </div>

                <p class='email-greeting'>
                    Желаем успехов в доработке и с нетерпением ждем исправленную версию!
                </p>
            </div>
        </div>

        <div class='email-footer'>
            <div class='email-contact'>
                <p><strong>С уважением,</strong></p>
                <p>Команда модерации «Рудзынг»</p>
            </div>
            
            <div class='email-support'>
                <p><strong>Контакты для связи:</strong></p>
                <p>Email: rudzyng@yandex.ru</p>
            </div>

            <div class='email-signature'>
                <p>© 2025 Рудзынг. Северо-Осетинский государственный университет</p>
            </div>
        </div>
    </div>
</body>
</html>";
        }

        private string BuildPublicationEmailHtml(string developerName, string gameName)
        {
            return $@"<!DOCTYPE html>
<html lang=""ru"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Игра опубликована!</title>
    <style>
        * {{
            padding: 0;
            margin: 0;
            border: 0;
            box-sizing: border-box;
        }}
        
        body {{
            background: #141C30;
            color: #F8FAFE;
            font-family: Arial, sans-serif;
            line-height: 1.6;
            margin: 0;
            padding: 0;
        }}
        
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 40px 20px;
        }}
        
        .email-header {{
            text-align: center;
            margin-bottom: 40px;
            padding-bottom: 20px;
            border-bottom: 2px solid #43495C;
        }}
        
        .email-logo {{
            font-family: Arial, sans-serif;
            font-size: 32px;
            color: #51D546;
            margin-bottom: 20px;
            font-weight: bold;
        }}
        
        .email-title {{
            font-size: 28px;
            font-weight: 600;
            color: #51D546;
            margin-bottom: 10px;
        }}
        
        .email-subtitle {{
            font-size: 18px;
            color: #666D80;
        }}
        
        .email-content {{
            background: #F8FAFE;
            border-radius: 10px 70px;
            padding: 40px;
            margin-bottom: 30px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
        }}
        
        .email-section {{
            margin-bottom: 30px;
        }}
        
        .email-section:last-child {{
            margin-bottom: 0;
        }}
        
        .email-greeting {{
            font-size: 16px;
            line-height: 1.6;
            color: #182032;
            margin-bottom: 20px;
        }}
        
        .email-success {{
            background: #FFFFFF;
            padding: 25px;
            border-radius: 10px 50px;
            margin: 25px 0;
            border-left: 4px solid #51D546;
        }}
        
        .email-success-title {{
            font-size: 18px;
            font-weight: 600;
            color: #182032;
            margin-bottom: 15px;
        }}       
        
        .email-next-steps {{
            background: #FFFFFF;
            padding: 20px;
            border-radius: 10px;
            margin: 20px 0;
        }}
        
        .email-next-title {{
            font-size: 18px;
            font-weight: 600;
            color: #182032;
            margin-bottom: 15px;
        }}
        
        .email-list {{
            margin: 15px 0;
            padding-left: 20px;
        }}
        
        .email-list-item {{
            position: relative;
            padding: 8px 0;
            color: #182032;
            line-height: 1.5;
        }}
        
        .email-list-item::before {{
            content: ""•"";
            color: #51D546;
            font-weight: bold;
            display: inline-block;
            width: 1em;
            margin-left: -1em;
        }}
        
        .email-note {{
            background: #FFFFFF;
            border-left: 4px solid #5B7FD9;
            padding: 20px;
            margin: 25px 0;
            border-radius: 0 10px 10px 0;
            color: #182032;
        }}
        
        .email-footer {{
            text-align: center;
            padding-top: 30px;
            border-top: 1px solid #43495C;
            color: #666D80;
        }}
        
        .email-contact {{
            margin: 20px 0;
            padding: 15px;
            background: #F8FAFE;
            border-radius: 10px;
        }}
        
        .email-support {{
            font-size: 14px;
            margin-top: 15px;
        }}
        
        .email-signature {{
            margin-top: 30px;
            color: #666D80;
        }}
        
        a {{
            color: #3F69D2;
            text-decoration: none;
        }}
        
        strong {{
            font-weight: 600;
        }}
        
        h3 {{
            font-size: 20px;
            font-weight: 600;
            color: #182032;
            margin-bottom: 20px;
        }}
        
        @media (max-width: 768px) {{
            .email-container {{
                padding: 20px 15px;
            }}
            
            .email-content {{
                padding: 30px 20px;
                border-radius: 10px 50px;
            }}
            
            .email-title {{
                font-size: 24px;
            }}
        }}
        
        @media (max-width: 480px) {{
            .email-content {{
                padding: 20px 15px;
            }}
            
            .email-success {{
                padding: 20px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""email-header"">
            <div class=""email-logo"">Рудзынг</div>
            <h1 class=""email-title"">Игра опубликована!</h1>
            <p class=""email-subtitle"">Поздравляем с успешной публикацией</p>
        </div>

        <div class=""email-content"">
            <div class=""email-section"">
                <p class=""email-greeting"">
                    Уважаемый(ая) <strong>{developerName}</strong>,
                </p>
                
                <div class=""email-success"">
                    <div class=""email-success-title"">🎉 Поздравляем!</div>
                    <p class=""email-greeting"" style=""margin-bottom: 0;"">
                        Ваша игра <strong>«{gameName}»</strong> успешно прошла модерацию и опубликована на платформе «Рудзынг».
                    </p>
                </div>
            </div>  

            <div class=""email-section"">
                <div class=""email-next-steps"">
                    <div class=""email-next-title"">Что дальше?</div>
                    <ul class=""email-list"">
                        <li class=""email-list-item"">
                            Рекомендуем поделиться ссылкой на игру в своих социальных сетях и сообществах
                        </li>
                        <li class=""email-list-item"">
                            Следите за отзывами и оценками игроков на нашей платформе — это лучший источник обратной связи
                        </li>
                    </ul>
                </div>

                <div class=""email-note"">
                    <p>
                        Мы восхищены вашей работой и желаем вашей игре высокой популярности!<br>
                        Не останавливайтесь на достигнутом — мы всегда рады вашим новым проектам.
                    </p>
                </div>
            </div>
        </div>

        <div class=""email-footer"">
            <div class=""email-contact"">
                <p><strong>С уважением и наилучшими пожеганиями,</strong></p>
                <p>Команда «Рудзынг»</p>
            </div>
            
            <div class=""email-support"">
                <p><strong>Контакты для связи:</strong></p>
                <p>Email: rudzyng@yandex.ru</p>
            </div>

            <div class=""email-signature"">
                <p>© 2025 Рудзынг. Северо-Осетинский государственный университет</p>
            </div>
        </div>
    </div>
</body>
</html>";
        }

        private string BuildRefusalGameEmailHtml(string developerName, string gameName)
        {
            return $@"<!DOCTYPE html>
<html lang=""ru"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Игра отклонена</title>
    <style>
        /* Базовые стили, которые лучше поддерживаются */
        body {{
            margin: 0;
            padding: 20px 0;
            background: #141C30;
            color: #F8FAFE;
            font-family: Arial, sans-serif;
            line-height: 1.6;
            -webkit-text-size-adjust: 100%;
        }}
        
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        
        /* Табличная верстка для лучшей поддержки */
        .email-table {{
            width: 100%;
            border-collapse: collapse;
        }}
        
        .email-header {{
            text-align: center;
            padding-bottom: 20px;
            border-bottom: 2px solid #43495C;
            margin-bottom: 30px;
        }}
        
        .email-logo {{
            font-family: Arial, sans-serif;
            font-size: 32px;
            color: #D54646;
            margin-bottom: 15px;
            font-weight: bold;
        }}
        
        .email-title {{
            font-size: 24px;
            font-weight: 600;
            color: #D54646;
            margin-bottom: 8px;
        }}
        
        .email-subtitle {{
            font-size: 16px;
            color: #666D80;
        }}
        
        .email-content {{
            background: #F8FAFE;
            border-radius: 10px;
            padding: 25px;
            margin-bottom: 20px;
        }}
        
        .email-greeting {{
            font-size: 16px;
            line-height: 1.6;
            color: #182032;
            margin-bottom: 15px;
        }}
        
        .email-rejection {{
            background: #FFFFFF;
            padding: 20px;
            border-radius: 8px;
            margin: 20px 0;
            border-left: 4px solid #D54646;
        }}
        
        .email-rejection-title {{
            font-size: 18px;
            font-weight: 600;
            color: #182032;
            margin-bottom: 10px;
        }}
        
        .email-reasons {{
            background: #FFF5F5;
            padding: 20px;
            border-radius: 8px;
            margin: 15px 0;
            border: 1px solid #FED7D7;
        }}
        
        .email-reasons-title {{
            font-size: 16px;
            font-weight: 600;
            color: #C53030;
            margin-bottom: 12px;
            text-align: center;
        }}
        
        /* Упрощенная верстка для пунктов */
        .email-reason-item {{
            padding: 10px 0;
            color: #182032;
            line-height: 1.5;
            border-bottom: 1px solid #FED7D7;
        }}
        
        .email-reason-item:last-child {{
            border-bottom: none;
        }}
        
        .email-reason-icon {{
            color: #D54646;
            display: inline-block;
            width: 20px;
        }}
        
        .email-final {{
            background: #FFFFFF;
            padding: 18px;
            border-radius: 6px;
            margin: 15px 0;
            border: 1px solid #E2E8F0;
        }}
        
        .email-final-title {{
            font-size: 16px;
            font-weight: 600;
            color: #182032;
            margin-bottom: 8px;
        }}
        
        .email-note {{
            background: #FFFFFF;
            border-left: 4px solid #718096;
            padding: 18px;
            margin: 20px 0;
            color: #182032;
        }}
        
        .email-footer {{
            text-align: center;
            padding-top: 25px;
            border-top: 1px solid #43495C;
            color: #666D80;
        }}
        
        .email-contact {{
            margin: 15px 0;
            padding: 12px;
            background: #F8FAFE;
            border-radius: 6px;
        }}
        
        .email-support {{
            font-size: 14px;
            margin-top: 12px;
        }}
        
        .email-signature {{
            margin-top: 20px;
            color: #666D80;
            font-size: 12px;
        }}
        
        /* Простые медиа-запросы для критичных изменений */
        @media only screen and (max-width: 480px) {{
            .email-container {{
                padding: 10px !important;
            }}
            
            .email-content {{
                padding: 15px !important;
            }}
            
            .email-title {{
                font-size: 20px !important;
            }}
            
            .email-logo {{
                font-size: 28px !important;
            }}
            
            .email-rejection,
            .email-reasons {{
                padding: 15px !important;
            }}
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""email-header"">
            <div class=""email-logo"">Рудзынг</div>
            <h1 class=""email-title"">Игра удалена</h1>
            <p class=""email-subtitle"">Уведомление от команды модерации</p>
        </div>

        <div class=""email-content"">
            <p class=""email-greeting"">
                Уважаемый(ая) <strong>{developerName}</strong>,
            </p>
            
            <p class=""email-greeting"">
                Спасибо за время и усилия, затраченные на создание игры <strong>«{gameName}»</strong> и ее отправку на нашу платформу.
            </p>

            <div class=""email-rejection"">
                <div class=""email-rejection-title"">❌ Игра отклонена</div>
                <p class=""email-greeting"" style=""margin-bottom: 0;"">
                    После тщательной проверки мы вынуждены сообщить, что игра была отклонена и не может быть опубликована на «Рудзынг».
                </p>
            </div>

            <div class=""email-reasons"">
                <div class=""email-reasons-title"">Возможна одна из причин отклонения:</div>
                
                <div class=""email-reason-item"">
                    <span class=""email-reason-icon"">♦️</span>
                    Игра содержит неприемлемый контент
                </div>
                
                <div class=""email-reason-item"">
                    <span class=""email-reason-icon"">♦️</span>
                    Техническое состояние игры не позволяет ей стабильно функционировать
                </div>
                
                <div class=""email-reason-item"">
                    <span class=""email-reason-icon"">♦️</span>
                    В игре не работает обязательный функционал, указанный в требованиях к играм для разработчиков
                </div>
            </div>

            <div class=""email-final"">
                <div class=""email-final-title"">Окончательное решение</div>
                <p class=""email-greeting"" style=""margin-bottom: 0;"">
                    Данное решение является окончательным и обжалованию не подлежит. К сожалению, мы не можем допустить к публикации контент, который нарушает наши правила или предоставляет негативный опыт для пользователей.
                </p>
            </div>

            <div class=""email-note"">
                <p>Мы понимаем, что это может быть неприятной новостью, и благодарим вас за понимание.</p>
            </div>
        </div>

        <div class=""email-footer"">
            <div class=""email-contact"">
                <p><strong>С уважением,</strong></p>
                <p>Команда модерации «Рудзынг»</p>
            </div>
            
            <div class=""email-support"">
                <p><strong>Контакты для связи:</strong></p>
                <p>Email: rudzyng@yandex.ru</p>
            </div>

            <div class=""email-signature"">
                <p>© 2025 Рудзынг. Северо-Осетинский государственный университет</p>
            </div>
        </div>
    </div>
</body>
</html>";
        }

        private string BuildGameUpdateNotificationEmailHtml(string devName, string devEmail, string gameName, int gameId)
        {
            return $@"<!DOCTYPE html>
<html lang=""ru"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Изменения в игре</title>
    <style>
        body {{
            margin: 0;
            padding: 20px 0;
            background: #141C30;
            color: #F8FAFE;
            font-family: Arial, sans-serif;
            line-height: 1.6;
            -webkit-text-size-adjust: 100%;
        }}
        
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        
        .email-header {{
            text-align: center;
            padding-bottom: 20px;
            border-bottom: 2px solid #43495C;
            margin-bottom: 30px;
        }}
        
        .email-logo {{
            font-family: Arial, sans-serif;
            font-size: 32px;
            color: #2E8B57;
            margin-bottom: 15px;
            font-weight: bold;
        }}
        
        .email-title {{
            font-size: 24px;
            font-weight: 600;
            color: #2E8B57;
            margin-bottom: 8px;
        }}
        
        .email-subtitle {{
            font-size: 16px;
            color: #666D80;
        }}
        
        .email-content {{
            background: #F8FAFE;
            border-radius: 10px;
            padding: 25px;
            margin-bottom: 20px;
        }}
        
        .email-greeting {{
            font-size: 16px;
            line-height: 1.6;
            color: #182032;
            margin-bottom: 15px;
        }}
        
        .email-notification {{
            background: #FFFFFF;
            padding: 20px;
            border-radius: 8px;
            margin: 20px 0;
            border-left: 4px solid #2E8B57;
        }}
        
        .email-notification-title {{
            font-size: 18px;
            font-weight: 600;
            color: #182032;
            margin-bottom: 10px;
        }}
        
        .email-dev-info {{
            background: #F0FFF4;
            padding: 20px;
            border-radius: 8px;
            margin: 15px 0;
            border: 1px solid #9AE6B4;
        }}
        
        .email-dev-info-title {{
            font-size: 16px;
            font-weight: 600;
            color: #2D3748;
            margin-bottom: 12px;
            text-align: center;
        }}
        
        .email-info-item {{
            padding: 10px 0;
            color: #182032;
            line-height: 1.5;
            border-bottom: 1px solid #E2E8F0;
        }}
        
        .email-info-item:last-child {{
            border-bottom: none;
        }}
        
        .email-info-label {{
            font-weight: 600;
            color: #2D3748;
            display: inline-block;
            width: 120px;
        }}
        
        .email-action {{
            background: #EBF8FF;
            padding: 18px;
            border-radius: 6px;
            margin: 15px 0;
            border: 1px solid #90CDF4;
        }}
        
        .email-action-title {{
            font-size: 16px;
            font-weight: 600;
            color: #2A4365;
            margin-bottom: 8px;
        }}
        
        .email-steps {{
            background: #FFFFFF;
            padding: 18px;
            margin: 20px 0;
            border-radius: 6px;
            border-left: 4px solid #4299E1;
        }}
        
        .email-step {{
            padding: 8px 0;
            color: #182032;
            display: flex;
            align-items: flex-start;
        }}
        
        .email-step-number {{
            background: #4299E1;
            color: white;
            border-radius: 50%;
            width: 24px;
            height: 24px;
            text-align: center;
            line-height: 24px;
            margin-right: 12px;
            flex-shrink: 0;
            font-size: 14px;
            font-weight: bold;
        }}
        
        .email-footer {{
            text-align: center;
            padding-top: 25px;
            border-top: 1px solid #43495C;
            color: #666D80;
        }}
        
        .email-contact {{
            margin: 15px 0;
            padding: 12px;
            background: #1E2A45;
            border-radius: 6px;
        }}
        
        .email-support {{
            font-size: 14px;
            margin-top: 12px;
        }}
        
        .email-signature {{
            margin-top: 20px;
            color: #666D80;
            font-size: 12px;
        }}
        
        strong {{
            font-weight: 600;
        }}
        
        @media only screen and (max-width: 480px) {{
            .email-container {{
                padding: 10px !important;
            }}
            
            .email-content {{
                padding: 15px !important;
            }}
            
            .email-title {{
                font-size: 20px !important;
            }}
            
            .email-logo {{
                font-size: 28px !important;
            }}
            
            .email-notification,
            .email-dev-info {{
                padding: 15px !important;
            }}
            
            .email-info-label {{
                width: 100px !important;
            }}
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""email-header"">
            <div class=""email-logo"">Рудзынг</div>
            <h1 class=""email-title"">Изменения в игре</h1>
            <p class=""email-subtitle"">Требуется проверка модератора</p>
        </div>

        <div class=""email-content"">
            <div class=""email-notification"">
                <div class=""email-notification-title"">📝 Обновление данных игры</div>
                <p class=""email-greeting"" style=""margin-bottom: 0;"">
                    Разработчик внес изменения в данные опубликованной игры.
                </p>
            </div>

            <div class=""email-dev-info"">
                <div class=""email-dev-info-title"">Информация о разработчике и игре</div>
                
                <div class=""email-info-item"">
                    <span class=""email-info-label"">Разработчик:</span>
                    <strong>{devName}</strong>
                </div>
                
                <div class=""email-info-item"">
                    <span class=""email-info-label"">Email:</span>
                    <strong>{devEmail}</strong>
                </div>
                
                <div class=""email-info-item"">
                    <span class=""email-info-label"">Игра:</span>
                    <strong>«{gameName}»</strong>
                </div>
                
                <div class=""email-info-item"">
                    <span class=""email-info-label"">ID игры:</span>
                    <strong>{gameId}</strong>
                </div>
            </div>

            <div class=""email-action"">
                <div class=""email-action-title"">🚀 Требуется действие</div>
                <p class=""email-greeting"" style=""margin-bottom: 0;"">
                    Пожалуйста, примите изменения или дайте обратную связь разработчику в случае несогласованности изменений с правилами платформы.
                </p>
            </div>

            <div class=""email-steps"">
                <div class=""email-step"">
                    <div class=""email-step-number"">1</div>
                    <div class=""email-greeting"" style=""margin-bottom: 0;"">
                        Проверьте обновленные данные игры в панели администратора
                    </div>
                </div>
                
                <div class=""email-step"">
                    <div class=""email-step-number"">2</div>
                    <div class=""email-greeting"" style=""margin-bottom: 0;"">
                        Убедитесь, что изменения соответствуют правилам платформы
                    </div>
                </div>
                
                <div class=""email-step"">
                    <div class=""email-step-number"">3</div>
                    <div class=""email-greeting"" style=""margin-bottom: 0;"">
                        Одобрите изменения или свяжитесь с разработчиком для уточнений
                    </div>
                </div>
            </div>

            <div style=""background: #FFFBF0; padding: 15px; border-radius: 6px; border: 1px solid #F6E05E; margin: 15px 0;"">
                <p style=""color: #744210; margin: 0; font-size: 14px; text-align: center;"">
                    ⏰ <strong>Рекомендуется обработать запрос в течение 24-48 часов</strong>
                </p>
            </div>
        </div>

        <div class=""email-footer"">
            <div class=""email-contact"">
                <p><strong>С уважением,</strong></p>
                <p>Команда платформы «Рудзынг»</p>
            </div>
            
            <div class=""email-support"">
                <p><strong>Панель администратора:</strong></p>
                <p><a href=""#"" style=""color: #63B3ED;"">Перейти к модерации</a></p>
            </div>

            <div class=""email-signature"">
                <p>© 2025 Рудзынг. Северо-Осетинский государственный университет</p>
            </div>
        </div>
    </div>
</body>
</html>";
        }

        private string BuildDeactivationEmailHtml(string developerName, string gameName)
        {
            return $@"<!DOCTYPE html>
<html lang=""ru"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Игра деактивирована</title>
    <style>
        * {{
            padding: 0;
            margin: 0;
            border: 0;
            box-sizing: border-box;
        }}
        
        body {{
            background: #141C30;
            color: #F8FAFE;
            font-family: Arial, sans-serif;
            line-height: 1.6;
            margin: 0;
            padding: 0;
        }}
        
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 40px 20px;
        }}
        
        .email-header {{
            text-align: center;
            margin-bottom: 40px;
            padding-bottom: 20px;
            border-bottom: 2px solid #43495C;
        }}
        
        .email-logo {{
            font-family: Arial, sans-serif;
            font-size: 32px;
            color: #FF6B6B;
            margin-bottom: 20px;
            font-weight: bold;
        }}
        
        .email-title {{
            font-size: 28px;
            font-weight: 600;
            color: #FF6B6B;
            margin-bottom: 10px;
        }}
        
        .email-subtitle {{
            font-size: 18px;
            color: #666D80;
        }}
        
        .email-content {{
            background: #F8FAFE;
            border-radius: 10px 70px;
            padding: 40px;
            margin-bottom: 30px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
        }}
        
        .email-section {{
            margin-bottom: 30px;
        }}
        
        .email-section:last-child {{
            margin-bottom: 0;
        }}
        
        .email-greeting {{
            font-size: 16px;
            line-height: 1.6;
            color: #182032;
            margin-bottom: 20px;
        }}
        
        .email-notice {{
            background: #FFFFFF;
            padding: 25px;
            border-radius: 10px 50px;
            margin: 25px 0;
            border-left: 4px solid #FF6B6B;
        }}
        
        .email-notice-title {{
            font-size: 18px;
            font-weight: 600;
            color: #182032;
            margin-bottom: 15px;
        }}       
        
        .email-next-steps {{
            background: #FFFFFF;
            padding: 20px;
            border-radius: 10px;
            margin: 20px 0;
        }}
        
        .email-next-title {{
            font-size: 18px;
            font-weight: 600;
            color: #182032;
            margin-bottom: 15px;
        }}
        
        .email-list {{
            margin: 15px 0;
            padding-left: 20px;
        }}
        
        .email-list-item {{
            position: relative;
            padding: 8px 0;
            color: #182032;
            line-height: 1.5;
        }}
        
        .email-list-item::before {{
            content: ""•"";
            color: #FF6B6B;
            font-weight: bold;
            display: inline-block;
            width: 1em;
            margin-left: -1em;
        }}
        
        .email-note {{
            background: #FFFFFF;
            border-left: 4px solid #5B7FD9;
            padding: 20px;
            margin: 25px 0;
            border-radius: 0 10px 10px 0;
            color: #182032;
        }}
        
        .email-footer {{
            text-align: center;
            padding-top: 30px;
            border-top: 1px solid #43495C;
            color: #666D80;
        }}
        
        .email-contact {{
            margin: 20px 0;
            padding: 15px;
            background: #F8FAFE;
            border-radius: 10px;
        }}
        
        .email-support {{
            font-size: 14px;
            margin-top: 15px;
        }}
        
        .email-signature {{
            margin-top: 30px;
            color: #666D80;
        }}
        
        a {{
            color: #3F69D2;
            text-decoration: none;
        }}
        
        strong {{
            font-weight: 600;
        }}
        
        h3 {{
            font-size: 20px;
            font-weight: 600;
            color: #182032;
            margin-bottom: 20px;
        }}
        
        @media (max-width: 768px) {{
            .email-container {{
                padding: 20px 15px;
            }}
            
            .email-content {{
                padding: 30px 20px;
                border-radius: 10px 50px;
            }}
            
            .email-title {{
                font-size: 24px;
            }}
        }}
        
        @media (max-width: 480px) {{
            .email-content {{
                padding: 20px 15px;
            }}
            
            .email-notice {{
                padding: 20px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""email-header"">
            <div class=""email-logo"">Рудзынг</div>
            <h1 class=""email-title"">Игра деактивирована</h1>
            <p class=""email-subtitle"">Важная информация о вашей игре</p>
        </div>

        <div class=""email-content"">
            <div class=""email-section"">
                <p class=""email-greeting"">
                    Уважаемый(ая) <strong>{developerName}</strong>,
                </p>
                
                <div class=""email-notice"">
                    <div class=""email-notice-title"">⚠️ Уведомление о деактивации</div>
                    <p class=""email-greeting"" style=""margin-bottom: 0;"">
                        Ваша игра <strong>«{gameName}»</strong> была деактивирована на платформе «Рудзынг».
                    </p>
                </div>
            </div>  

            <div class=""email-section"">
                <div class=""email-next-steps"">
                    <div class=""email-next-title"">Что делать дальше?</div>
                    <ul class=""email-list"">
                        <li class=""email-list-item"">
                            Вы можете создать новую версию игры с учетом наших рекомендаций
                        </li>
                        <li class=""email-list-item"">
                            Если у вас есть вопросы, свяжитесь с нашей службой поддержки, ответив на это письмо
                        </li>
                    </ul>
                </div>

                <div class=""email-note"">
                    <p>
                        Мы ценим ваш вклад в развитие осетинского языка и надеемся на дальнейшее сотрудничество.<br>
                        Наша команда готова помочь вам в создании новых образовательных проектов.
                    </p>
                </div>
            </div>
        </div>

        <div class=""email-footer"">
            <div class=""email-contact"">
                <p><strong>С уважением,</strong></p>
                <p>Команда «Рудзынг»</p>
            </div>
            
            <div class=""email-support"">
                <p><strong>Контакты для связи:</strong></p>
                <p>Email: rudzyng@yandex.ru</p>
                <p style=""margin-top: 10px;"">
                    <small>Если у вас есть вопросы, ответьте на это письмо</small>
                </p>
            </div>

            <div class=""email-signature"">
                <p>© 2025 Рудзынг. Северо-Осетинский государственный университет</p>
            </div>
        </div>
    </div>
</body>
</html>";
        }
    }
}
