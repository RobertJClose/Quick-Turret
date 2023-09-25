using QuickTurret.TargetingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITaggable
{
    public TagsAsset TagsAsset { get; }
    public List<TargetingTag> Tags { get; }
}
