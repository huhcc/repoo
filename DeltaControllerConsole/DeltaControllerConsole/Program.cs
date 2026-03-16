using DeltaControllerConsole.Cnt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltaControllerConsole
{
    internal class Program
    {
        static CntDeltaManager deltaManager;
        static void Main(string[] args)
        {
            Console.WriteLine("Приложение");
           

            string ip = "";
            int port = 0;
            int readingDelay = 0;

            Console.WriteLine($"Подключение к {ip}:{port}...");

            try
            {
                // Менеджер
                deltaManager = new CntDeltaManager(ip, port, readingDelay);

                // Кнопки
                if (deltaManager.DeltaController != null)
                {
                    deltaManager.DeltaController.NotifyKeyOnOffOn += (msg) =>
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Нажата кнопка ВКЛ/ВЫКЛ");

                    deltaManager.DeltaController.NotifyKeySelectOn += (msg) =>
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Нажата кнопка ВЫБОР");

                    deltaManager.DeltaController.NotifyKeyCancelOn += (msg) =>
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Нажата кнопка ОТМЕНА");

                    deltaManager.DeltaController.NotifyKeyKKOpen += (msg) =>
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Нажата кнопка КК");
                }

                Console.WriteLine("Подключение выполнено успешно");
                Console.WriteLine("\nКоманды:");
                Console.WriteLine("  r - Показать текущие значения");
                Console.WriteLine("  w [адрес] [значение] - Записать значение по адресу");
                Console.WriteLine("  e - Выход из программы");
                Console.WriteLine();

                // Сама прога
                bool running = true;
                while (running)
                {
                    string input = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(input)) continue;

                    string[] parts = input.Split(' ');

                    switch (parts[0].ToLower())
                    {
                        case "r":
                            ReadData();
                            break;

                        case "w":
                            if (parts.Length == 3 && int.TryParse(parts[1], out int addr) && int.TryParse(parts[2], out int val))
                            {
                                deltaManager.SetDeltaControllerData(addr, val);
                                Console.WriteLine($"Записано значение {val} по адресу {addr}");
                            }
                            else
                            {
                                Console.WriteLine("Неверная команда. Используйте: w [адрес] [значение]");
                            }
                            break;

                        case "e":
                            running = false;
                            break;

                        default:
                            Console.WriteLine("Неизвестная команда");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.WriteLine($"Стек вызовов: {ex.StackTrace}");
            }
            finally
            {
                if (deltaManager?.DeltaController != null)
                {
                    deltaManager.DeltaController.close();
                }
                Console.WriteLine("Программа завершена");
            }
        }

        static void ReadData()
        {
            if (deltaManager?.cntDeltaData != null)
            {
                Console.WriteLine("\n--- Текущие значения ---");
                Console.WriteLine($"Счетчик: {deltaManager.cntDeltaData.Counter}");
                Console.WriteLine($"Метры: {deltaManager.cntDeltaData.Meters}");
                Console.WriteLine($"Номера катушек: {deltaManager.cntDeltaData.CoilNumbers}");
                Console.WriteLine($"Сброс тревоги: {deltaManager.cntDeltaData.Reset_Alarm_Success}");

                if (deltaManager.cntDeltaData.bitsButton != null)
                {
                    Console.WriteLine("Состояние кнопок:");
                    Console.WriteLine($"  ВКЛ/ВЫКЛ: {deltaManager.cntDeltaData.bitsButton[0]}");
                    Console.WriteLine($"  ВЫБОР: {deltaManager.cntDeltaData.bitsButton[1]}");
                    Console.WriteLine($"  ОТМЕНА: {deltaManager.cntDeltaData.bitsButton[2]}");
                    Console.WriteLine($"  КК: {deltaManager.cntDeltaData.bitsButton[3]}");
                }
                Console.WriteLine("------------------------\n");
            }
            else
            {
                Console.WriteLine("Нет данных для отображения");
            }
        }
    }
}
