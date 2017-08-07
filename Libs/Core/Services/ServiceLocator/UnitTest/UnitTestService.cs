namespace MMGame
{
    /// <summary>
    /// 本测试需要 Service 以 的方式存在于场景中。
    /// </summary>
    public class UnitTestService : UnitTest
    {
        [TestError(0)]
        public int TestRegisterNullType()
        {
            ServiceLocator.Register(null, new ServiceTestAAA());
            return LineNumber;
        }

        [TestError(0)]
        public int TestRegisterNullInstance()
        {
            ServiceLocator.Register<IServiceTestAAA>(null);
            return LineNumber;
        }

        [TestError(0)]
        public int TestRegisterInvalidCast()
        {
            ServiceLocator.Register<IServiceTestAAA>(new ServiceTestBBB());
            return LineNumber;
        }

        [TestMethod(0)]
        public void TestRegister()
        {
            ServiceLocator.Register<IServiceTestAAA>(new ServiceTestAAA());
            IsNotNull(ServiceLocator.Get<IServiceTestAAA>());
        }

        [TestMethod(0)]
        public void TestRegisterTwice()
        {
            ServiceLocator.Register<IServiceTestAAA>(new ServiceTestAAA());
            // 检查是否有提示信息输出。
        }

        [TestMethod(0)]
        public void Unregister()
        {
            ServiceLocator.Unregister<IServiceTestAAA>();
            IsFalse(ServiceLocator.Has<IServiceTestAAA>());
        }

        [TestError(0)]
        public int UnregisterNonExistType()
        {
            ServiceLocator.Unregister<IServiceTestCCC>();
            return LineNumber;
        }

        [TestError(0)]
        public int TestGetNonExistType()
        {
            ServiceLocator.Get<IServiceTestCCC>();
            return LineNumber;
        }

        [TestMethod(0)]
        public void TestGet()
        {
            ServiceLocator.Register<IServiceTestBBB>(new ServiceTestBBB());
            IsNotNull(ServiceLocator.Get<IServiceTestBBB>());
        }

        [TestMethod(0)]
        public void TestClearAndHas()
        {
            ServiceLocator.Register<IServiceTestAAA>(new ServiceTestAAA());
            ServiceLocator.Register<IServiceTestCCC>(new ServiceTestCCC());
            ServiceLocator.Clear();
            IsFalse(ServiceLocator.Has<IServiceTestAAA>());
            IsFalse(ServiceLocator.Has<IServiceTestBBB>());
            IsFalse(ServiceLocator.Has<IServiceTestCCC>());

            ServiceLocator.Register<IServiceTestCCC>(new ServiceTestCCC());
            IsTrue(ServiceLocator.Has<IServiceTestCCC>());
        }
    }

    // ------------------------------------------------------
    // 测试用接口和类
    // ------------------------------------------------------

    public interface IServiceTestAAA
    {
    }

    public class ServiceTestAAA : IServiceTestAAA
    {
    }

    public interface IServiceTestBBB
    {
    }

    public class ServiceTestBBB : IServiceTestBBB
    {
    }

    public interface IServiceTestCCC
    {
    }

    public class ServiceTestCCC : IServiceTestCCC
    {
    }
}