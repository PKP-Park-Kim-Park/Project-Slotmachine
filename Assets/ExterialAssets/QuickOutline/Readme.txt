Quick Outline
=============

Developed by Chris Nolet (c) 2018


Instructions
------------

To add an outline to an object, drag-and-drop the Outline.cs
script onto the object. The outline materials will be loaded
at runtime.

You can also add outlines programmatically with:

    var outline = gameObject.AddComponent<Outline>();

    outline.OutlineMode = Outline.Mode.OutlineAll;
    outline.OutlineColor = Color.yellow;
    outline.OutlineWidth = 5f;

The outline script does a small amount of work in Awake().
For best results, use outline.enabled to toggle the outline.
Avoid removing and re-adding the component if possible.

For large meshes, you may also like to enable 'Precompute
Outline' in the editor. This will reduce the amount of work
performed in Awake().


Troubleshooting
---------------

If the outline appears off-center, please try the following:

1. Set 'Read/Write Enabled' on each model's import settings.
2. Disable 'Optimize Mesh Data' in the player settings.

----------------------------------------------------------------------------
물체의 외곽선 적용 방법
1. Outline.cs 스크립트를 물체에 드래그 앤 드롭합니다.
2. Outline.cs 스크립트의 속성을 조정하여 외곽선 모드, 색상 및 너비를 설정합니다.
3. 외곽선은 런타임에 자동으로 적용됩니다.
4. 외곽선 스크립트를 꺼서 처음에 비활성화 시키고 바라봤을때 외곽선이 적용되도록 할 수 있습니다.
----------------------------------------------------------------------------
