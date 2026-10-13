# Practical Examination 1/2569 - Course 958321 Game Development 3
- **Name:** Thaneth Hamburut (นายธเณศร์ เห็มบุรุษ)
- **Student Code:** 672110095
- **Section:** 1
- **Namespace:** `ThanethHamburut`
---
## 📌 Commit History & Description (คำอธิบายแต่ละ Commit)
| ลำดับ | Commit Message | รายละเอียดสิ่งที่ทำ (Commit Comment / Description) |
| :---: | :--- | :--- |
| **1** | `Initial commit` | สร้างโปรเจกต์ Unity 6 ตั้งต้น พร้อมตั้งค่า `.gitignore` และ `.gitattributes` สำหรับ Unity |
| **2** | `feat: Complete Part 1 Scenes Management and Flow...` | **(Day 1 - Q.1)** สร้างและเชื่อมต่อฉากทั้ง 8 ฉาก (`MainMenu`, `StageSelection`, `Stage1`, `Stage2`, `Options`, `Credits`, `Win`, `GameOver`) ใน Build Settings และเขียน `SceneNavigationManager.cs` |
| **3** | `Implement Scene Navigation, Capsule Player, ProBuilder...` | **(Day 2 - Q.2, Q.3, Q.4, Q.5, Q.9-Q.12)** สร้าง `CapsulePlayerController`, กล้อง `SmoothCameraFollow`, ออกแบบฉาก Stage 1 ด้วย ProBuilder, สร้าง Item Prefabs 4 ชนิด, ระบบนับเวลา 30 วิ / คะแนน 10,000 แต้ม (`Stage1Manager`) และระบบเสียง `AudioManager` (BGM/SFX/Volume Sliders) |
| **4** | `Configure Humanoid Player with Starter Assets...` | **(Day 3 - Q.6)** ตั้งค่าโมเดล `Ch02_nonPBR.fbx` เป็น Humanoid Rig เชื่อมต่อกับ Starter Assets Third Person Controller ใส่เสียงก้าวเท้า และติดตั้ง `PlayerInteractionDetector.cs` |
| **5** | `Day 4: Implement Q.7 Stage 2 ProBuilder Level Design...` | **(Day 4 - Q.7)** ออกแบบฉาก Stage 2 ด้วย ProBuilder พื้นที่รวมกว่า 700 ตร.ม. (เกินเกณฑ์ 50 ตร.ม.) พร้อมกระจาย Item Prefabs 37 ชิ้นทั่วแผนที่ และจุดตกเหว `FallZone` |
| **6** | `Day 5: Complete Q.8 Stage 2 interaction system...` | **(Day 5 - Q.8)** พัฒนาระบบ Interaction กดปุ่ม `E` เก็บไอเทมแทนการเดินชน (`IInteractable`), สร้าง `Stage2Manager.cs`, จัดหน้าจอ HUD ขนาดใหญ่อ่านง่าย และปรับปรุง Performance (Static Batching, GPU Instancing, Zero-alloc) |
| **7** | `fix` | **(Hotfix)** แก้ไขถาวรปัญหาโมเดลหุ่นยนต์ซ้อนทับกับตัวละคร Ch02, ลบ `InitializeOnLoadMethod` ใน Editor Script และปลด `CinemachineBrain` เพื่อให้กล้อง `SmoothCameraFollow` ติดตามตัวละครใน Stage 2 ได้อย่างสมบูรณ์ |
