# SCP-420-J: Smoking Edition

https://plugins.data-dlx.pro/p/BPQ5zH_-WsgV

Превращает SCP-420-J в сигареты: +15 HP, временный буст скорости и риск умереть от рака лёгких.

## Особенности

- **Сохранён оригинальный эффект:** +15 к максимальному здоровью и эффект «Rainbow».
- **Курение:** отнимает 5 HP и даёт +15% к скорости передвижения на 12 секунд.
- **Смертельный исход:** если закурить 5 сигарет подряд, персонаж мгновенно умирает с причиной «рак лёгких».
- **Полностью серверная логика:** не требует дополнительных сетевых сообщений или синхронизации.
- **Совместимость:** SCP:CS 1.1.

## Установка

1. Поместите файлы `SCP420J.cs` и `PluginInitializator.cs` в папку `Plugins` на сервере.
2. Убедитесь, что в этой же сборке есть **ровно один** класс `PluginInfo` (например, ваш `Info`).
3. Перезапустите сервер — он автоматически скомпилирует все `.cs` файлы в `ServerPlugins.dll` и зарегистрирует предмет.

> Если вы используете готовую DLL, просто положите её в `Plugins` и перезапустите сервер.

## Документация: PluginInitializator (SCP:CS 1.1)

`PluginInitializator` — это точка входа для вашего плагина. Игра вызывает `InitServer()` на сервере и `InitClient()` на клиенте. Внутри этих методов вы регистрируете все игровые классы (предметы, классы игроков, панели и т.д.).

**Пример из официальной документации:**

```csharp
using Akequ.Plugins;
using Plugin.Rooms;

namespace PluginExample
{
    internal class Initializator : PluginInitializator
    {
        public override void InitClient()
        {
            PluginAPI.RegisterPlayerClass(typeof(CustomPlayer), false);
            PluginAPI.RegisterItem(typeof(CustomItem), false);
            PluginAPI.RegisterAdminPanel(typeof(CustomPanel), "permission");
            PluginAPI.RegisterRoomEvent(typeof(CustomRoom));
        }

        public override void InitServer()
        {
            PluginAPI.RegisterPlayerClass(typeof(CustomPlayer), false);
            PluginAPI.RegisterItem(typeof(CustomItem), false);
            PluginAPI.RegisterAdminPanel(typeof(CustomPanel), "permission");
            PluginAPI.RegisterRoomEvent(typeof(CustomRoom));
            HookManager.Add("onMapGenerationComplete", (obj) => {
                CustomRoom map = new CustomRoom();
                PluginAPI.SpawnNetworkedEvent(map);
            });
        }
    }
}