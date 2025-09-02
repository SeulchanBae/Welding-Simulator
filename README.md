# Welding Simulator (2025 Graduation Project)

Unity 기반 **Meta Quest 3 MR 용접 시뮬레이터**  
2025년도 졸업작품으로 제작 중이며, MR 환경에서 용접 훈련을 체험할 수 있도록 개발됩니다.

---

##  프로젝트 개요
- **플랫폼**: Meta Quest 3 (Standalone)
- **Unity 버전**: 2022.3.60f1
- **렌더 파이프라인**: URP
- **XR SDK**: Meta XR All-in-One SDK + OpenXR
- **목표 기능**:
  - MR 공간 인식 및 작업 환경 구성
  - 용접 대상 선택 & 배치
  - 용접 시뮬레이션 (점수/결과 분석 포함)
  - 가이드/튜토리얼 모드

---

##  폴더 구조

```
Assets/
├─ project_Merge/
│ ├─ Scenes/ # 개인별 담당 씬
│ │ ├─ scn_Boot.unity # 부트스트랩 씬 (Additive 로딩)
│ │ ├─ scn_Menu.unity # 메뉴/UI
│ │ ├─ scn_Environment.unity # 환경/장비
│ │ ├─ scn_Welding.unity # 용접 로직/이펙트
│ │ └─ scn_UI.unity # 공통 오버레이 UI
│ ├─ Scripts/
│ │ ├─ Runtime/ # 런타임 스크립트
│ │ └─ Editor/ # 에디터 전용 스크립트
│ ├─ Prefabs/ # 공용 프리팹
│ ├─ Materials/ # 머티리얼
│ ├─ Audio/ # 사운드
│ └─ Addressables/ # (선택) 에셋 번들 관리
├─ ThirdParty/ # 외부 패키지/플러그인
```

---

##  개발 환경 설정

### 1. 필수 설치
- **Unity Hub**
- **Unity 2022.3.60f1** (Android Build Support + OpenJDK + NDK 포함)
- **Git LFS**
  ```bash
  git lfs install
  ```

### 2. 레포 클론
```bash
git clone https://github.com/SeulchanBae/Welding-Simulator.git
```

### 3. Unity Hub에서 열기
- 프로젝트 폴더를 Unity Hub에 추가 후 실행

### 4. XR 설정 확인
- **Edit → Project Settings → XR Plug-in Management**
- **Android 탭**: OpenXR 활성
- **Oculus Touch Controller**, **Meta Quest Support** 체크

### 5. 빌드 테스트
- **File → Build Settings → Android**로 전환
- Quest 3 연결 후 **Build And Run**

---

## 팀 작업 규칙
- **씬 단위 작업 분리**: 각 팀원은 자기 담당 씬만 수정
- **공용 리소스**: `Prefabs` 폴더 사용
- **Git 규칙**: `.gitignore`와 `Git LFS` 규칙 준수 (대용량 파일은 LFS에 자동 추적됨)
