using System;
using System.Collections.Generic;
using System.Linq;
using Client;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Client.SystemEnum;
using Object = UnityEngine.Object;

public class SynergyUITests
{
    private Dictionary<eSynergy, Dictionary<int, List<SynergyData>>> _originalData;
    private GameObject _panel;

    [SetUp]
    public void SetUp()
    {
        var map = DataManager.Instance.SynergyDataMap;
        _originalData = new Dictionary<eSynergy, Dictionary<int, List<SynergyData>>>(map);
        map.Clear();
        foreach (var group in new SynergyData().LoadData().Values.Cast<SynergyData>().GroupBy(data => data.synergy))
            map.Add(group.Key, group.GroupBy(data => data.synergyCount)
                .ToDictionary(level => level.Key, level => level.ToList()));
    }

    [TearDown]
    public void TearDown()
    {
        if (_panel) Object.DestroyImmediate(_panel);
        var map = DataManager.Instance.SynergyDataMap;
        map.Clear();
        foreach (var pair in _originalData) map.Add(pair.Key, pair.Value);
    }

    [Test]
    public void DuplicateCharactersCountAsOneKind()
    {
        var container = new SynergyContainer(eSynergy.SWORD, eCharType.ALLY);
        container.Register(Member(100, 1, eSynergy.SWORD));
        container.Register(Member(100, 2, eSynergy.SWORD));
        Assert.That(container.MemberCount, Is.EqualTo(2));
        Assert.That(new SynergyDisplayInfo(container).DistinctMembers, Is.EqualTo(1));
        Assert.That(new SynergyDisplayInfo(container).IsActive, Is.False);

        container.Register(Member(200, 3, eSynergy.SWORD));
        var display = new SynergyDisplayInfo(container);
        Assert.That(display.DistinctMembers, Is.EqualTo(2));
        Assert.That(display.IsActive, Is.True);
        Assert.That(display.ActiveThresholds, Is.EqualTo(new[] { 2, 3, 4 }));
    }

    [TestCase(0, false, 0)]
    [TestCase(1, true, 1)]
    [TestCase(2, false, 2)]
    [TestCase(3, false, 3)]
    [TestCase(4, true, 4)]
    [TestCase(5, true, 4)]
    public void LaplacianUsesExactSelectedTier(int count, bool active, int threshold)
    {
        var display = Display(eSynergy.LAPLACIAN, count);
        Assert.That(display.IsActive, Is.EqualTo(active));
        Assert.That(display.CurrentThreshold, Is.EqualTo(threshold));
        Assert.That(display.ActiveThresholds, Is.EqualTo(new[] { 1, 4 }));
    }

    [Test]
    public void MultipleEffectsShareOneDisplayEntryAndThreshold()
    {
        var display = Display(eSynergy.TIMER, 4);
        Assert.That(display.CurrentThreshold, Is.EqualTo(4));
        Assert.That(display.ActiveThresholds, Is.EqualTo(new[] { 2, 3, 4 }));
        Assert.That(display.IsActive, Is.True);
    }

    [Test]
    public void TeamPanelsRenderSeparatelyAndReuseRows()
    {
        var prefab = Resources.Load<GameObject>("Prefabs/UI/Scene/UI_GameSceneSynergyTab");
        Assert.That(prefab, Is.Not.Null);
        _panel = Object.Instantiate(prefab);
        var teamA = _panel.transform.Find("TeamA");
        var teamB = _panel.transform.Find("TeamB");

        MessageManager.SendMessage(new OnSynergyChange(eCharType.ALLY,
            new[] { Display(eSynergy.SWORD, 2), Display(eSynergy.LAPLACIAN, 2) }));
        MessageManager.SendMessage(new OnSynergyChange(eCharType.ENEMY,
            new[] { Display(eSynergy.TIMER, 3) }));

        Assert.That(teamA.GetComponentsInChildren<SynergyUnitUI>().Length, Is.EqualTo(2));
        Assert.That(teamB.GetComponentsInChildren<SynergyUnitUI>().Length, Is.EqualTo(1));
        var sword = teamA.GetComponentsInChildren<SynergyUnitUI>()[0];
        Assert.That(sword.transform.Find("SynergyLevel").GetComponent<TMP_Text>().text, Is.EqualTo("2"));
        Assert.That(sword.transform.Find("SynergyStep").GetComponent<TMP_Text>().text, Does.Contain("<u>2</u>"));
        Assert.That(sword.transform.Find("SynergyIcon").GetComponent<Image>().enabled, Is.False);

        MessageManager.SendMessage(new OnSynergyChange(eCharType.ALLY, Array.Empty<SynergyDisplayInfo>()));
        Assert.That(teamA.GetComponentsInChildren<SynergyUnitUI>(), Is.Empty);
        Assert.That(teamB.GetComponentsInChildren<SynergyUnitUI>().Length, Is.EqualTo(1));
        MessageManager.SendMessage(new OnSynergyChange(eCharType.ALLY, new[] { Display(eSynergy.SHIELD, 1) }));
        Assert.That(teamA.GetComponentsInChildren<SynergyUnitUI>()[0], Is.SameAs(sword));
        Assert.That(teamA.GetComponentsInChildren<SynergyUnitUI>(true).Length, Is.EqualTo(2));

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)teamA);
        Assert.That(sword.GetComponent<RectTransform>().rect.width, Is.GreaterThan(0));
        Assert.That(_panel.GetComponent<CanvasScaler>().referenceResolution, Is.EqualTo(new Vector2(3840, 2160)));
    }

    private static SynergyDisplayInfo Display(eSynergy synergy, int count)
    {
        var container = new SynergyContainer(synergy, eCharType.ALLY);
        for (int i = 0; i < count; i++) container.Register(Member(100 + i, i, synergy));
        return new SynergyDisplayInfo(container);
    }

    private static CharLightWeightInfo Member(long index, long uid, eSynergy synergy)
    {
        return new CharLightWeightInfo
        {
            Index = index, Uid = uid, Side = eCharType.ALLY,
            SynergyList = new List<eSynergy> { synergy }
        };
    }
}
