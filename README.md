# Silly Seal

A 3D game about a seal that waddles along a beach and swims through the water.

This is a Unity 6 (URP) project using the new **Input System** package
(`Assets/InputSystem_Actions.inputactions`, `Player` action map).

## Scene: SampleScene

`Assets/Scenes/SampleScene.unity` is pre-wired with a playable placeholder setup:

- **Beach** — a scaled Plane (100x100) at the origin, walkable ground.
- **Water** — a scaled Cube (60x4x60) overlapping the far edge of the beach,
  with a trigger `BoxCollider` and `WaterVolume`. Entering/exiting it toggles
  the seal between land and swim movement.
- **Seal** — a Capsule with a `CharacterController` and `SealController`,
  already wired to the `InputSystem_Actions` asset.
- **Main Camera** — has `FollowCamera` targeting the Seal.

All the geometry is default Unity primitives with no materials/art yet — swap
in real meshes, terrain, and a water shader whenever you're ready; the
scripts don't care what the visuals look like, only the collider shapes.

To build this setup again from scratch (e.g. in a new scene), replicate the
above: Plane for ground, a trigger volume + `WaterVolume` for water, a
`CharacterController` + `SealController` for the seal (drag the
`InputSystem_Actions` asset into its `Input Actions` field — Move/Jump/Crouch
are resolved by name from the `Player` action map at runtime), and
`FollowCamera` on the Main Camera targeting the seal's transform.

## Controls

(from the `Player` action map in `InputSystem_Actions`)

- `WASD` / arrow keys / left stick — move (camera-relative)
- `Space` / gamepad south button — jump (on land) / swim up (in water)
- `C` / gamepad east button — dive down (in water)

## Project structure

```
Assets/
  Scripts/
    Player/       SealController.cs   – land + swim movement, water state
    Environment/  WaterVolume.cs      – trigger volume that toggles swim state
    Camera/       FollowCamera.cs     – simple smoothed third-person follow cam
  Scenes/         (add your .unity scenes here)
  Prefabs/        (add Seal / environment prefabs here)
  Materials/
  Art/
```

## Next steps / ideas

- Add an Animator Controller for the seal (the scripts already drive `Speed`
  and `IsSwimming` animator parameters if an `Animator` is assigned).
- Add buoyancy/surface-clamping so the seal bobs at the water surface instead
  of swimming at a fixed depth.
- Add collectibles (fish) and simple beach obstacles.
