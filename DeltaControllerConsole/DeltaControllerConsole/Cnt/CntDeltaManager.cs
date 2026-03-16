using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeltaControllerConsole.Cnt
{
    public class CntDeltaManager
    {
        public cntDelta DeltaController;
        private Thread thread_load_cnt;
        private int _deltaControllerReadingDelay;
        public cntDeltaData cntDeltaData = new cntDeltaData();
        private string _deltaControllerConnectionIP;
        private double _deltaControllerConnectionPort;


        /// <summary>
        /// Класс для взаимодействия с контроллером.
        /// <c><see cref="SetDeltaControllerData"/></c> - запись данных в контроллер.
        /// <c><see cref="DeltaControllerDataReading"/> </c> - циклично считывает данные из контроллера, поток запускается при инициализации.
        /// </summary>
        /// <param name="DeltaControllerConnectionIP"></param>
        /// <param name="DeltaControllerConnectionPort"></param>
        /// <param name="DeltaControllerReadingDelay"></param>
        public CntDeltaManager(string DeltaControllerConnectionIP, double DeltaControllerConnectionPort, double DeltaControllerReadingDelay)
        {

            _deltaControllerConnectionIP = DeltaControllerConnectionIP;
            _deltaControllerConnectionPort = DeltaControllerConnectionPort;
            _deltaControllerReadingDelay = (int)DeltaControllerReadingDelay;

            DeltaControllerInit();
        }
        public void DeltaControllerInit()
        {
            DeltaController = new cntDelta(_deltaControllerConnectionIP, (int)_deltaControllerConnectionPort);
            DeltaController.open();

            thread_load_cnt = new Thread(new ThreadStart(DeltaControllerDataReading))
            {
                IsBackground = true
            };
            thread_load_cnt.Start();
        }

        public void GetDeltaControllerData()
        {

        }

        /// <summary>
        /// Метод для записи значений в переменные контроллера
        /// Адрес перменной берётся из 
        /// <c> <see cref="СntDeltaModbus"/> </c>, может также быть указан в виде int
        /// Значение переменной указывается вручную
        /// </summary>
        /// <param name="Adress"></param>
        /// <param name="Value"></param>
        public void SetDeltaControllerData(int Adress, int Value)
        {
            string owner = System.Reflection.MethodBase.GetCurrentMethod().Name;
            DeltaController.writeInt(Adress, Value, owner);
        }


        /// <summary>
        /// Метод потока для считывания данных из контроллера
        /// Считывание производится с задержкой 
        /// <c><see cref="DeltaControllerDataReading"/></c>
        /// Запись полученных данных идет в 
        /// <c><see cref="cntDeltaData"/></c>
        /// Для добавлениях переменных в метод считывания, необходимо указать их в
        /// <seealso cref="cntDelta.read"/>
        /// </summary>
        private void DeltaControllerDataReading()
        {

            while (true)
            {
                Thread.Sleep(_deltaControllerReadingDelay);
                //DELTA
                try
                {
                    DeltaController.read(cntDeltaData);

                }
                catch
                {

                }

            }
        }
    }
}
