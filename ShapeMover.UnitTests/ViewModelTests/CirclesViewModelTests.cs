using Moq;
using ShapeMover.Helpers.Interfaces;
using ShapeMover.WPF.ViewModels;
using System.Windows;

namespace ShapeMover.UnitTests.ViewModelTests;

[TestClass]
public class CirclesViewModelTests
{
    [TestMethod]
    public void AddCircle_SingleCircle_CircleAdded()
    {
        Mock<IRandomGenerator> mockRandomGenerator = new Mock<IRandomGenerator>();
        mockRandomGenerator.SetupSequence(x => x.Generate(It.IsAny<int>()))
            .Returns(100)
            .Returns(200);

        CirclesViewModel testClass = new CirclesViewModel(mockRandomGenerator.Object);

        testClass.AddCircle();

        Assert.AreEqual(1, testClass.Circles.Count);
        Assert.AreEqual(100, testClass.Circles[0].X);
        Assert.AreEqual(200, testClass.Circles[0].Y);
    }

    [TestMethod]
    public void AddCircle_MultipleCircles_CirclesAdded()
    {
        Mock<IRandomGenerator> mockRandomGenerator = new Mock<IRandomGenerator>();
        mockRandomGenerator.SetupSequence(x => x.Generate(It.IsAny<int>()))
            .Returns(100)
            .Returns(200)
            .Returns(300)
            .Returns(350);

        CirclesViewModel testClass = new CirclesViewModel(mockRandomGenerator.Object);

        testClass.AddCircle();
        testClass.AddCircle();

        Assert.AreEqual(2, testClass.Circles.Count);
        Assert.AreEqual(100, testClass.Circles[0].X);
        Assert.AreEqual(200, testClass.Circles[0].Y);
        Assert.AreEqual(300, testClass.Circles[1].X);
        Assert.AreEqual(350, testClass.Circles[1].Y);
    }

    [TestMethod]
    public void MoveCircle_SingleCircle_CircleMoved()
    {
        Mock<IRandomGenerator> mockRandomGenerator = new Mock<IRandomGenerator>();
        mockRandomGenerator.Setup(x => x.Generate(It.IsAny<int>())).Returns(100);

        CirclesViewModel testClass = new CirclesViewModel(mockRandomGenerator.Object);

        testClass.AddCircle();
        testClass.MoveCircle(0, new Point(10, 11));

        Assert.AreEqual(1, testClass.Circles.Count);
        Assert.AreEqual(10, testClass.Circles[0].X);
        Assert.AreEqual(11, testClass.Circles[0].Y);
    }

    [TestMethod]
    public void Undo_AddCircle_UndoSuccessful()
    {
        Mock<IRandomGenerator> mockRandomGenerator = new Mock<IRandomGenerator>();
        mockRandomGenerator.Setup(x => x.Generate(It.IsAny<int>())).Returns(100);

        CirclesViewModel testClass = new CirclesViewModel(mockRandomGenerator.Object);

        testClass.AddCircle();
        testClass.Undo();

        Assert.AreEqual(0, testClass.Circles.Count);
    }

    [TestMethod]
    public void Redo_AddCircle_RedoSuccessful()
    {
        Mock<IRandomGenerator> mockRandomGenerator = new Mock<IRandomGenerator>();
        mockRandomGenerator.Setup(x => x.Generate(It.IsAny<int>())).Returns(100);

        CirclesViewModel testClass = new CirclesViewModel(mockRandomGenerator.Object);

        testClass.AddCircle();
        testClass.Undo();
        testClass.Redo();

        Assert.AreEqual(1, testClass.Circles.Count);
        Assert.AreEqual(100, testClass.Circles[0].X);
        Assert.AreEqual(100, testClass.Circles[0].Y);
    }

    [TestMethod]
    public void Undo_MoveCircle_UndoSuccessful()
    {
        Mock<IRandomGenerator> mockRandomGenerator = new Mock<IRandomGenerator>();
        mockRandomGenerator.Setup(x => x.Generate(It.IsAny<int>())).Returns(100);

        CirclesViewModel testClass = new CirclesViewModel(mockRandomGenerator.Object);

        testClass.AddCircle();
        testClass.MoveCircle(0, new Point(200, 300));
        testClass.Undo();

        Assert.AreEqual(1, testClass.Circles.Count);
        Assert.AreEqual(100, testClass.Circles[0].X);
        Assert.AreEqual(100, testClass.Circles[0].Y);
    }

    [TestMethod]
    public void Redo_MoveCircle_RedoSuccessful()
    {
        Mock<IRandomGenerator> mockRandomGenerator = new Mock<IRandomGenerator>();
        mockRandomGenerator.Setup(x => x.Generate(It.IsAny<int>())).Returns(100);

        CirclesViewModel testClass = new CirclesViewModel(mockRandomGenerator.Object);

        testClass.AddCircle();
        testClass.MoveCircle(0, new Point(200, 300));
        testClass.Undo();
        testClass.Redo();

        Assert.AreEqual(1, testClass.Circles.Count);
        Assert.AreEqual(200, testClass.Circles[0].X);
        Assert.AreEqual(300, testClass.Circles[0].Y);
    }

    [TestMethod]
    public void Redo_HistoryTruncated_NoRedo()
    {
        Mock<IRandomGenerator> mockRandomGenerator = new Mock<IRandomGenerator>();
        mockRandomGenerator.Setup(x => x.Generate(It.IsAny<int>())).Returns(100);

        CirclesViewModel testClass = new CirclesViewModel(mockRandomGenerator.Object);

        testClass.AddCircle();
        testClass.AddCircle(); //undo back to here
        testClass.AddCircle();
        testClass.MoveCircle(0, new Point(200, 300));
        testClass.MoveCircle(0, new Point(350, 450));
        testClass.MoveCircle(1, new Point(555, 444));

        //Undo back to third state (including first empty state)
        testClass.Undo();
        testClass.Undo();
        testClass.Undo();
        testClass.Undo();

        testClass.AddCircle();

        //these redos will have no effect since we did AddCirle last
        testClass.Redo();
        testClass.Redo();
        testClass.Redo();

        Assert.AreEqual(3, testClass.Circles.Count);
        Assert.AreEqual(100, testClass.Circles[0].X);
        Assert.AreEqual(100, testClass.Circles[0].Y);
        Assert.AreEqual(100, testClass.Circles[1].X);
        Assert.AreEqual(100, testClass.Circles[1].Y);

        //note that circle with ID 2 was cut from the history, the last circle added has ID 3
        Assert.AreEqual(100, testClass.Circles[3].X);
        Assert.AreEqual(100, testClass.Circles[3].Y);
    }

}
