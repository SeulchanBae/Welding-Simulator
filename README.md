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

---

## 🔥 용접 시뮬레이션 시스템

### 점수 평가 시스템

용접 작업은 **4가지 카테고리**로 평가되며, 각 항목은 **100점 만점**에서 감점 방식으로 채점됩니다.

#### 1. 정확도 (Accuracy)
- **초기 점수**: 100점
- **감점 조건**: 용접봉이 메탈플레이트(잘못된 위치)에 닿을 때
- **감점**: 접촉당 -5점
- **평가 목적**: 정확한 용접 위치 선정 능력

#### 2. 숙련도 (Proficiency)
- **초기 점수**: 100점
- **감점 조건**: 전체 용접 작업이 20초를 초과할 때
- **감점**: 1초 초과당 -5점
- **평가 목적**: 빠르고 효율적인 작업 능력

#### 3. 깊이 (Depth)
- **초기 점수**: 100점
- **감점 조건**: 용접봉이 백보드(너무 깊은 위치)에 닿을 때
- **감점**: 접촉당 -5점
- **평가 목적**: 적절한 용접 깊이 유지 능력

#### 4. 품질 (Quality)
- **초기 점수**: 100점
- **감점 조건**: 가이드라인마다 부적절한 시간 동안 용접할 때
  - 너무 빠른 용접: **0.3초 미만**
  - 너무 느린 용접: **1.0초 초과**
- **감점**: 불량 용접당 -5점
- **평가 목적**: 일정한 속도와 품질 유지 능력

---

### 용접 메커니즘 (시간 기반 연속 접촉 시스템)

실제 용접과 유사하게 **연속적인 접촉 시간**을 측정하는 방식을 채택했습니다.

#### 1. 동적 가이드 감지
```csharp
// GameManager.cs:46
private void InitializeWeldingGuides()
{
    WeldingGuideColorChanger[] guides = FindObjectsOfType<WeldingGuideColorChanger>();
    totalGuideCount = guides.Length;
    Debug.Log($"용접 가이드 초기화 완료: 총 {totalGuideCount}개의 가이드 발견");
}
```
- 씬 로드 시 모든 `WeldingGuide` 오브젝트를 **자동으로 감지**
- 프리팹마다 가이드 개수가 달라도 **동적으로 조정**
- "66개 고정" 문제 해결 → 유연한 시뮬레이션 구성 가능

#### 2. 용접 프로세스 흐름
```
① 용접봉 접촉 (OnTriggerEnter)
    ↓
② 시간 측정 시작 (StartWelding)
    ↓
③ 용접봉 이탈 (OnTriggerExit)
    ↓
④ 경과 시간 계산 (EndWelding)
    ↓
⑤ 품질 평가 (WeldGuide)
    ↓
⑥ 색상 변경 (Green/Yellow)
```

#### 3. 색상 코드 시스템

| 색상 | 상태 | 접촉 시간 | 설명 |
|------|------|-----------|------|
| 🔴 **빨강** | 미용접 | - | 아직 용접하지 않은 가이드 |
| 🟢 **초록** | 완벽 | 0.3초 ~ 1.0초 | 적절한 속도로 용접 완료 |
| 🟡 **노랑** | 불량 | < 0.3초 or > 1.0초 | 너무 빠르거나 느린 용접 (품질 -5점) |

#### 4. 완료 조건
```csharp
// GameManager.cs:99
public void OnGuideWelded()
{
    weldedGuideCount++;
    if (weldedGuideCount >= totalGuideCount)
    {
        EndWeldingSession(); // 결과 씬으로 전환
    }
}
```
- **모든 가이드**가 용접 완료(초록 또는 노랑)될 때 자동 종료
- 결과 화면(`scn_result`)으로 전환되어 **4가지 점수** 표시

---

### Unity 설정 가이드

#### 필수 태그 생성

Unity Editor에서 다음 태그를 생성해야 합니다:

| 태그 이름 | 용도 | 대상 오브젝트 |
|-----------|------|---------------|
| `WeldingGuide` | 용접 가이드라인 | 가이드 큐브들 |
| `MetalPlate` | 잘못된 접촉 영역 | 메탈플레이트 |
| `Backboard` | 너무 깊은 용접 | 백보드 |

**태그 생성 방법**:
1. Unity Editor → **Tags & Layers**
2. **Tags** 섹션 → **"+"** 버튼 클릭
3. 각 태그 이름 입력 후 **Save**

#### Prefab 설정

**1. metalPlatePrefab 설정**

**가이드라인 큐브들**:
- Tag: `WeldingGuide`
- Component: `WeldingGuideColorChanger.cs` 추가
- Material 할당:
  - **Green Material**: `WeldingGuide_Green.mat`
  - **Yellow Material**: `WeldingGuide_Yellow.mat`
  - **기본 Material**: `WeldingGuide_Red.mat`
- **Collider**: `Is Trigger` 체크 필수

**메탈플레이트**:
- Tag: `MetalPlate`
- **Collider**: `Is Trigger` 체크 필수

**백보드**:
- Tag: `Backboard`
- **Collider**: `Is Trigger` 체크 필수

**2. 용접봉 (Welder) 설정**:
- Component: `WeldingSparkController.cs` 추가
- Particle System: 용접 스파크 파티클 할당
- **Collider**: `Is Trigger` 체크 필수

#### 용접 시간 파라미터 조정

`WeldingGuideColorChanger.cs`의 Inspector에서 조정 가능:

```csharp
[Header("용접 시간 설정")]
public float minWeldingTime = 0.3f;  // 최소 용접 시간 (초)
public float maxWeldingTime = 1.0f;  // 최대 용접 시간 (초)
```

- 난이도에 따라 시간 범위 조정 가능
- 예: Easy (0.2~1.5초), Normal (0.3~1.0초), Hard (0.4~0.8초)

---

### 주요 스크립트 구조

#### 1. GameManager.cs
> `Assets/Merge_File/Scripts/GameManager.cs`

**핵심 기능**:
- Singleton 패턴 (`DontDestroyOnLoad`)
- 4가지 점수 추적 및 관리
- 동적 가이드 개수 감지 (`:46`)
- 용접 완료 추적 (`:99`)

**주요 메서드**:
| 메서드 | 설명 | 위치 |
|--------|------|------|
| `InitializeWeldingGuides()` | 씬의 모든 가이드 자동 감지 | :46 |
| `OnGuideWelded()` | 가이드 용접 완료 시 호출 | :99 |
| `OnWrongContact()` | 정확도 감점 (-5점) | :112 |
| `OnBackboardContact()` | 깊이 감점 (-5점) | :106 |
| `OnPoorQuality()` | 품질 감점 (-5점) | :117 |
| `EndWeldingSession()` | 결과 화면 전환 | :128 |

#### 2. WeldingGuideColorChanger.cs
> `Assets/Merge_File/Scripts/WeldingGuideColorChanger.cs`

**핵심 기능**:
- 각 가이드의 용접 시간 측정
- 시간 기반 품질 평가 및 색상 변경

**주요 메서드**:
| 메서드 | 설명 | 위치 |
|--------|------|------|
| `StartWelding()` | 용접 시작 시간 기록 | :76 |
| `EndWelding()` | 경과 시간 반환 | :86 |
| `WeldGuide(float duration)` | 시간 평가 및 색상 결정 | :31 |

#### 3. WeldingSparkController.cs
> `Assets/Merge_File/Scripts/WeldingSparkController.cs`

**핵심 기능**:
- 용접봉의 충돌 감지
- 스파크 효과 재생

**주요 메서드**:
| 메서드 | 설명 | 위치 |
|--------|------|------|
| `OnTriggerEnter()` | 용접 시작 및 스파크 효과 | :7 |
| `OnTriggerExit()` | 용접 종료 및 평가 | :45 |

---

### 디버깅 팁

게임 실행 중 **Console 로그**를 확인하면 다음 정보를 실시간으로 볼 수 있습니다:

```
용접 가이드 초기화 완료: 총 66개의 가이드 발견
용접 완료: 1 / 66
완벽한 용접! 시간: 0.75초
너무 빠른 용접! 시간: 0.15초 (최소: 0.3초)
정확도 점수 감점! 현재 점수: 95
깊이 점수 감점! 현재 점수: 95
품질 점수 감점! 현재 점수: 95
용접 완료! 경과 시간: 18.45초
최종 정확도 점수: 95
최종 숙련도 점수: 100
최종 깊이 점수: 95
최종 품질 점수: 90
```

---

### 향후 개발 계획

- [ ] 다양한 용접 패턴 추가 (직선, 곡선, 원형)
- [ ] 난이도 조절 시스템 (Easy/Normal/Hard)
- [ ] 리더보드 및 기록 저장
- [ ] 실시간 음향 효과 개선
- [ ] 햅틱 피드백 강화

---
