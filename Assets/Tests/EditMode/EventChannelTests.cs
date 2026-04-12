using NUnit.Framework;
using UnityEngine;
using GameSystemsCookbook;

namespace PaddleBall.Tests
{
    [TestFixture]
    public class EventChannelTests
    {
        private VoidEventChannelSO m_VoidChannel;

        [SetUp]
        public void SetUp()
        {
            m_VoidChannel = ScriptableObject.CreateInstance<VoidEventChannelSO>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(m_VoidChannel);
        }

        [Test]
        public void VoidEventChannel_RaiseEvent_NotifiesSubscriber()
        {
            bool wasInvoked = false;
            m_VoidChannel.OnEventRaised += () => wasInvoked = true;

            m_VoidChannel.RaiseEvent();

            Assert.IsTrue(wasInvoked);
        }

        [Test]
        public void VoidEventChannel_NoSubscribers_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => m_VoidChannel.RaiseEvent());
        }

        [Test]
        public void VoidEventChannel_MultipleSubscribers_NotifiesAll()
        {
            int callCount = 0;
            m_VoidChannel.OnEventRaised += () => callCount++;
            m_VoidChannel.OnEventRaised += () => callCount++;

            m_VoidChannel.RaiseEvent();

            Assert.AreEqual(2, callCount);
        }
    }
}
