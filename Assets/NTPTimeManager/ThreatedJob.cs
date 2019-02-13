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

        //static CancellationTokenSource tokenSource2 = new CancellationTokenSource();
        //static CancellationToken ct = tokenSource2.Token;
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
                    endTime = Time.unscaledTime + abordTime;
                    started = true;
                    task.Start();
                }
                bool isTimeCompleted = Time.unscaledTime >= endTime;
                if (isTimeCompleted) {
                    started = false;
                    isSuccess = false;
                    return false;
                }
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
                timeManager.TryToGetTime();
                while(!timeManager.timeReceived) {
                    time = timeManager.milliseconds;
                    isSuccess = timeManager.timeReceived;
                }
                Debug.Log("TIME RECEIVED!");
                timeManager.timeReceived = false;
            });
                //Task.Run(() => time = timeManager.TryToGetTime(out isSuccess));
#endif
        }

    }

    public class ThreatedJob<T> : CustomYieldInstruction
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
        public bool isSuccess = true;
        public T data;

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
                return true;
#endif
            }
        }

        public ThreatedJob(ThreatedEvent<T> job, float abordTime = 2)
        {
            this.abordTime = abordTime;
#if UNITY_EDITOR
            ThreadStart thre = new ThreadStart(delegate
            {
                data = job.Invoke(out isSuccess);
            });
            thread = new Thread(thre);
#endif
#if !UNITY_EDITOR

#endif
        }

    }


    public class ThreatedJob : CustomYieldInstruction
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
        public bool isSuccess = true;

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
                return true;
#endif
            }
        }

        public ThreatedJob(ThreatedEvent job, float abordTime = 2)
        {
            this.abordTime = abordTime;
#if UNITY_EDITOR
            ThreadStart thre = new ThreadStart(delegate
            {
                job.Invoke(out isSuccess);
            });
            thread = new Thread(thre);
#endif
#if !UNITY_EDITOR

#endif
        }

    }

    public delegate T ThreatedEvent<T>(out bool success);
    public delegate void ThreatedEvent(out bool success);

}