//Copyright(c) 2017 �mer Faruk Say�l�r

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using UnityEngine;

#if UNITY_EDITOR
using System.Threading;
#endif

#if !UNITY_EDITOR
using System.Threading;
using System.Threading.Tasks;
#endif

namespace UnbiasedTimeManager
{

    public class GetNetworkTime : CustomYieldInstruction
    {
#if UNITY_EDITOR
        Thread thread;
#endif

#if !UNITY_EDITOR
        Task task;
#endif

        float abordTime;
        float endTime;
        bool started = false;
        public bool isSuccess = false;
        public ulong time;

        public override bool keepWaiting
        {
            get
            {
#if UNITY_EDITOR
                if (!started)
                {
                    endTime = Time.unscaledTime + abordTime;
                    started = true;
                    thread.Start();
                }
                bool isTimeCompleted = Time.unscaledTime >= endTime;
                if (isTimeCompleted)
                {
                    started = false;
                    isSuccess = false;
                    thread.Abort();
                    return false;
                }
                return thread.IsAlive;
#endif
#if !UNITY_EDITOR
                if (!started) {
                    //endTime = Time.unscaledTime + abordTime;
                    started = true;
                    task.Start();
                }
                //bool isTimeCompleted = Time.unscaledTime >= endTime;
                //if (isTimeCompleted) {
                //    task.Wait();
                //    started = false;
                //    isSuccess = false;
                //    return false;
                //}
                return !task.IsCompleted;
#endif
            }
        }

        public GetNetworkTime(UnbiasedTime timeManager, float abordTime = 3)
        {
            this.abordTime = abordTime;

#if UNITY_EDITOR
            ThreadStart threadStart = new ThreadStart(delegate
            {
                time = timeManager.TryToGetTime(out isSuccess);
            });
            thread = new Thread(threadStart);
#endif
#if !UNITY_EDITOR
            task = new Task(() => {
                Debug.Log("TASK STARTED");
                timeManager.TryToGetTime();
                while(!timeManager.timeReceived) {
                    Debug.Log("Waiting for time...." + isSuccess);
                }
                time = timeManager.milliseconds;
                isSuccess = timeManager.timeReceived;

                Debug.Log("TIME RECEIVED!");
                timeManager.timeReceived = false;
            });
#endif
        }

    }
}