﻿// Copyright (c) 2012-2019 Vaughn Vernon. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading;
using Vlingo.Common;
using Vlingo.Actors.TestKit;

namespace Vlingo.Actors.Tests.Supervision
{
    public class FailureControlActor : Actor, IFailureControl
    {
        public static ThreadLocal<FailureControlActor> Instance = new ThreadLocal<FailureControlActor>();
        private readonly FailureControlTestResults testResults;

        public FailureControlActor(FailureControlTestResults testResults)
        {
            this.testResults = testResults;
            Instance.Value = this;
        }

        public void AfterFailure()
        {
            testResults.AfterFailureCount.IncrementAndGet();
            testResults.UntilAfterFail.Happened();
        }

        public void AfterFailureCount(int count)
        {
            testResults.AfterFailureCount.IncrementAndGet();
            testResults.UntilFailureCount.Happened();
        }

        public void FailNow()
        {
            testResults.FailNowCount.IncrementAndGet();
            testResults.UntilFailNow.Happened();
            throw new ApplicationException("Intended failure.");
        }

        protected internal override void BeforeStart()
        {
            testResults.BeforeStartCount.IncrementAndGet();
            testResults.UntilFailNow.Happened();
            base.BeforeStart();
        }

        protected internal override void AfterStop()
        {
            testResults.AfterStopCount.IncrementAndGet();
            testResults.UntilFailNow.Happened();
            base.AfterStop();
        }

        protected internal override void BeforeRestart(Exception reason)
        {
            testResults.BeforeRestartCount.IncrementAndGet();
            testResults.UntilFailNow.Happened();
            base.BeforeRestart(reason);
        }

        protected internal override void AfterRestart(Exception reason)
        {
            base.AfterRestart(reason);
            testResults.AfterRestartCount.IncrementAndGet();
            testResults.UntilAfterRestart.Happened();
        }

        protected internal override void BeforeResume(Exception reason)
        {
            testResults.BeforeResume.IncrementAndGet();
            testResults.UntilBeforeResume.Happened();
            base.BeforeResume(reason);
        }

        public override void Stop()
        {
            testResults.StoppedCount.IncrementAndGet();
            testResults.UntilStopped.Happened();
            base.Stop();
        }

        public class FailureControlTestResults
        {
            public AtomicInteger AfterFailureCount = new AtomicInteger(0);
            public AtomicInteger AfterFailureCountCount = new AtomicInteger(0);
            public AtomicInteger AfterRestartCount = new AtomicInteger(0);
            public AtomicInteger AfterStopCount = new AtomicInteger(0);
            public AtomicInteger BeforeRestartCount = new AtomicInteger(0);
            public AtomicInteger BeforeResume = new AtomicInteger(0);
            public AtomicInteger BeforeStartCount = new AtomicInteger(0);
            public AtomicInteger FailNowCount = new AtomicInteger(0);
            public AtomicInteger StoppedCount = new AtomicInteger(0);

            public TestUntil UntilAfterFail = TestUntil.Happenings(0);
            public TestUntil UntilAfterRestart = TestUntil.Happenings(0);
            public TestUntil UntilBeforeResume = TestUntil.Happenings(0);
            public TestUntil UntilFailNow = TestUntil.Happenings(0);
            public TestUntil UntilFailureCount = TestUntil.Happenings(0);
            public TestUntil UntilStopped = TestUntil.Happenings(0);
        }
    }
}
