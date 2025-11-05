🌍 Location-Based AR Object Spawner (Unity + Cesium + ARCore)

This project demonstrates a location-based Augmented Reality (AR) system built in Unity that allows users to input Latitude, Longitude, and Altitude coordinates — and spawns a 3D object exactly at that real-world position using Cesium for Unity and Google ARCore.

🚀 Overview

The project combines Cesium’s real-world 3D globe with AR Foundation and ARCore Extensions to bridge GPS coordinates and Unity’s 3D coordinate system.

Goal:
When a user types latitude and longitude into the app and clicks Submit, a prefab object appears in the AR view at the exact corresponding Earth position (georeferenced).

🧩 Core Features

✅ User inputs Latitude, Longitude, and Altitude in Unity UI
✅ CesiumGeoreference converts GPS → Earth → Unity coordinates
✅ Object spawns at the correct real-world location
✅ Works both in Editor (simulation) and on ARCore-supported Android devices
✅ Optional AR anchor stabilizes object in the physical world

🏗️ Architecture & Components
1. CesiumGeoreference

The bridge between real-world coordinates (WGS84 system) and Unity’s coordinate space.
It defines where on Earth your Unity world’s origin (0,0,0) is located.

Converts Longitude/Latitude/Height → Earth-Centered, Earth-Fixed (ECEF) coordinates.

Converts ECEF → Unity world coordinates for accurate positioning.

2. AR Foundation + ARCore Extensions

Provides:

Device pose tracking

Real-world plane detection

Optional ARAnchor components to keep spawned objects stable

3. CesiumEllipsoid

Handles Earth shape and geodetic math.
Used to compute ECEF coordinates from lat/lon/height.

4. UI Input

A simple Unity UI Canvas with:

Input fields for Latitude, Longitude, and Altitude

A Submit button to trigger the spawn

🧠 How It Works (Step-by-Step)
1️⃣ User Input

The user enters Latitude, Longitude, and (optional) Altitude values in the input fields.

Example:

Latitude: 37.4221
Longitude: -122.0841
Altitude: 10.0

2️⃣ Conversion Process

The script converts coordinates through three systems:

Step	Coordinate System	Purpose
A	Geographic (Lat/Lon/Alt)	User input (WGS84)
B	ECEF (Earth-Centered, Earth-Fixed)	3D Cartesian space around Earth center
C	Unity Local (x, y, z)	Unity’s coordinate system in meters

Code:

double3 ecef = georeference.ellipsoid.LongitudeLatitudeHeightToCenteredFixed(
    new double3(longitude, latitude, altitude));

double3 unityDouble = georeference.TransformEarthCenteredEarthFixedPositionToUnity(ecef);
Vector3 unityPos = new Vector3((float)unityDouble.x, (float)unityDouble.y, (float)unityDouble.z);

3️⃣ ARSessionOrigin Alignment

If AR Foundation is active, Unity transforms the spawned object to match the AR camera’s tracking origin:

Vector3 finalPos = sessionOrigin.transform.TransformPoint(unityPos);

4️⃣ Object Spawning

A prefab is instantiated at the computed world position:

var anchorGO = new GameObject("GeoAnchor");
anchorGO.transform.position = finalPos;
Instantiate(prefab, anchorGO.transform);


Optionally, an ARAnchor is added for positional stability on mobile devices.

⚙️ Prerequisites
🧰 Tools
Tool	Version
Unity Editor	2022.3+ (LTS)
Cesium for Unity	Latest (via Package Manager or GitHub)
AR Foundation	6.0+
ARCore XR Plugin	6.0+
ARCore Extensions	1.40.0+
🌐 APIs & SDKs

Google ARCore API

Handles device tracking and AR anchors.

Installed via Unity Package Manager or ARCore Extensions GitHub
.

Cesium for Unity

Streams real-world 3D terrain and provides coordinate conversion utilities.

Requires Cesium ion account (optional for base maps).

Unity XR + AR Foundation

Provides platform-agnostic AR session and camera tracking.

🪄 Usage Instructions
1️⃣ Scene Setup

Add these objects:

AR Session

AR Session Origin (with AR Camera)

CesiumGeoreference

Canvas UI with:

3 Input Fields (Latitude, Longitude, Altitude)

1 Button (“Submit”)

Create a new GameObject → attach CesiumGeoSpawner.cs.

2️⃣ Assign References

In the Inspector:

Assign:

CesiumGeoreference

Prefab to spawn

UI elements

Leave ARSession and ARSessionOrigin blank (they’ll be auto-detected).

3️⃣ Enter Play Mode or Build

Enter Latitude, Longitude, and Altitude.

Click Submit.

Your prefab appears in the real-world mapped position (in Cesium globe or AR view).

🧩 Script Summary
// Converts Lat/Lon to Unity coordinates
double3 ecef = georeference.ellipsoid.LongitudeLatitudeHeightToCenteredFixed(
    new double3(longitude, latitude, altitude));

double3 unityDouble = georeference.TransformEarthCenteredEarthFixedPositionToUnity(ecef);
Vector3 unityPos = new Vector3((float)unityDouble.x, (float)unityDouble.y, (float)unityDouble.z);
Instantiate(prefab, unityPos, Quaternion.identity);

📡 How It Spawns in the Real World

User Input: Real GPS coordinates (WGS84).

Cesium Conversion:

Converts WGS84 → ECEF (Cartesian around Earth).

Converts ECEF → Unity world meters using CesiumGeoreference origin.

Unity Rendering: Object appears exactly where that coordinate lies on the Cesium globe.

ARCore Tracking:

Keeps device pose aligned with physical environment.

Object remains fixed relative to real-world location.

🔍 Debug Tips

If the object spawns too far away → check if your CesiumGeoreference origin is set near your test location.

If it’s below terrain → raise the altitude by +5 to +10 meters.

Use Debug.Log() to print unityPos for verifying conversions.
