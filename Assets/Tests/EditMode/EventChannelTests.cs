using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
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

            Assert.That(wasInvoked, Is.True);
        }

        [Test]
        public void VoidEventChannel_NoSubscribers_DoesNotThrow()
        {
            Assert.That(() => m_VoidChannel.RaiseEvent(), Throws.Nothing);
        }

        [Test]
        public void VoidEventChannel_MultipleSubscribers_NotifiesAll()
        {
            int callCount = 0;
            m_VoidChannel.OnEventRaised += () => callCount++;
            m_VoidChannel.OnEventRaised += () => callCount++;

            m_VoidChannel.RaiseEvent();

            Assert.That(callCount, Is.EqualTo(2));
        }

        [Test]
        public void VoidEventChannel_UnsubscribedListener_IsNotNotified()
        {
            bool wasInvoked = false;
            UnityAction listener = () => wasInvoked = true;

            m_VoidChannel.OnEventRaised += listener;
            m_VoidChannel.OnEventRaised -= listener;
            m_VoidChannel.RaiseEvent();

            Assert.That(wasInvoked, Is.False);
        }
    }
}
