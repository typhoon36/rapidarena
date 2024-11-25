# [Unity 3D] RapidArena



## 게임 소개

- 장르 : FPS 3D MutiPlayer(P2P)

- 개발기간 (2024.10.01 → 2024.11.23)

- 목적  : FPS 멀티플레이 게임을 구현해보고자 제작한 프로젝트입니다.
  
- 관리 : Github/Jira

## 개발 환경
- 플랫폼 : Windows 11

- 언어 : C#

- 엔진 환경 : Unity 2022.03.15(LTS)

## 구현 기능
* UI
   * Scene
      * Scene  : 시작 , 종료 버튼,설정버튼
      * InGame : HP 
   * WorldSPace
      * 닉네임


## 사용 기술

| 항목 | 설명 |
| ------------ | ------------- |
| 디자인 패턴 | 싱글톤 패턴을 사용해서 전역 접근 관리 & State Pattern을 사용해 캐릭터 애니메이션을 객체 관리|
| Object Pooling | 오브젝트 풀링 기법을 사용해 자주 쓰는 객체를 미리 저장하고 생성 |
| Save | 게임내 데이터를 글로벌 변수에 저장 및 관리 |
| 포톤| 포톤으로 P2P 팀 대전을 관리|

 ## 기술 문서
[기술 문서](https://docs.google.com/presentation/d/1ASBFL0deqHO50BUfyGx-w9X3S-U0FXeQ4qpRaYTWGsU/edit?usp=sharing)

 ## velog

[블로그](https://velog.io/@typhoon760/posts?tag=%ED%8F%AC%ED%8A%B8%ED%8F%B4%EB%A6%AC%EC%98%A4)

 ## 영상
[플레이 영상](https://youtu.be/7IL2NW_SoXI?si=WMTmHV96WeVGsZX_)
 
  
