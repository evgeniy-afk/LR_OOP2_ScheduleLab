ЛАБОРАТОРНА РОБОТА №2* — РОБОТА З ФАЙЛАМИ XML + GOOGLE DRIVE
Варіант 5 «Розклад»

ВМІСТ АРХІВУ
  ScheduleLab.sln        — рішення, відкривайте його у Visual Studio
  ScheduleLab\           — проєкт .NET MAUI (Windows)
  SampleData\            — Schedule.xml і Schedule.xsl для перевірки

ЯК ЗАПУСТИТИ

1. Розпакуйте архів у папку без дуже довгого шляху (наприклад, C:\Labs\).

2. Отримайте credentials.json для Google Drive:
   а) зайдіть на https://console.cloud.google.com і створіть проєкт;
   б) APIs & Services -> Library -> Google Drive API -> Enable;
   в) OAuth consent screen (Google Auth Platform) -> External,
      вкажіть назву і свою пошту, у Test users додайте свою Gmail-адресу;
   г) Credentials -> Create credentials -> OAuth client ID -> Desktop app;
   д) завантажте JSON, перейменуйте на credentials.json
      і покладіть у папку ScheduleLab\Resources\Raw\

3. Відкрийте ScheduleLab.sln у Visual Studio
   (потрібен компонент ".NET Multi-platform App UI development").

4. Проєкт налаштований на .NET 10. Якщо у вас .NET 8 або .NET 9 і з'являється
   помилка про версію, відкрийте ScheduleLab.csproj і в рядку TargetFrameworks
   замініть net10.0 на net8.0 або net9.0.

5. Угорі оберіть "Windows Machine" і натисніть запуск (F5).
   Перший запуск довший: Visual Studio завантажує NuGet-пакети.

6. При першому зверненні до Google Drive відкриється браузер для входу.
   Попередження "Google hasn't verified this app" -> Continue.

ДЕ ЩО ЗБЕРІГАЄТЬСЯ
  Журнал подій (Одинак):     Документи\ScheduleLab\log.txt
  HTML після трансформації:  Документи\ScheduleLab\*.html
  Файли на Google Drive:     у корені вашого Drive

ДЕ РЕАЛІЗОВАНІ ВИМОГИ
  SAX / DOM / LINQ to XML         Strategies\SaxSearchStrategy.cs, DomSearchStrategy.cs, LinqSearchStrategy.cs
  Шаблон «Стратегія»              Strategies\ISearchStrategy.cs + SearchContext.cs
  Динамічна генерація запитів     BuildXPath (DOM), ланцюжок Where (LINQ)
  Підгрузка значень атрибутів     Services\XmlValuesLoader.cs
  XSL -> HTML                     Services\HtmlTransformer.cs + SampleData\Schedule.xsl
  Підтвердження виходу, Clear     Services\AppExit.cs, MauiProgram.cs, ClearCommand
  Без «магічної кнопки»           MVVM: ViewModels\MainViewModel.cs, кнопки лише викликають команди
  Google Drive                    Services\GoogleDriveService.cs
  Шаблон «Фабричний метод»        Saving\FragmentSavers.cs (CreateWriter), Saving\FragmentWriters.cs
  Шаблон «Одинак», 3 рівні подій  Logging\Logger.cs
