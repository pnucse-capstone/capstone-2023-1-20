# 10조 캡스톤 프로젝트

## 1. 프로젝트 소개

**제목** 일반화 된 다중 입력 라인형 리듬 액션 게임 시스템

이 졸업 과제는 위와 같은 라인형 리듬 게임들의 시스템을 일반화한 시스템을 구현하는 것을 목표로 한다. 기존 리듬 게임 시스템을 일반화한 이 조건들을 모두 만족시키는 시스템을 제안하고, 그러한 시스템을 구현하는 것을 목표로 한다.

## 2. 팀 소개

201924464 문예강 mygm1302@gmail.com
201424418 김세영

## 3. 구성도

### 유저 시나리오

Scene은 네가지 Scene이 존재하여 아래와 같이 전환될 수 있다.
![Score](https://github.com/pnucse-capstone/capstone-2023-1-10/assets/60247136/ebb9bb04-184f-49d2-85ad-ee98e08f0728)

### Intro Scene

게임 시작 직후에 진입하는 화면으로, Music Select Scene으로 이동할 수 있다.
![인트로](https://github.com/pnucse-capstone/capstone-2023-1-10/assets/60247136/c5f1b342-f803-4e6b-9b5c-516534789f77)

### Music Select Scene

플레이할 음악을 선택하고 난이도를 선택할 수 있는 화면이다. Intro Scene으로 되돌아가거나 선택한 레벨에 대해 In Game Scene으로 진입할 수 있다.
![선곡](https://github.com/pnucse-capstone/capstone-2023-1-10/assets/60247136/bb51ffff-0a13-4ef5-b7a5-90f70795bbd4)

### In Game Scene

실제 게임 플레이가 이루어진다. 음악의 끝까지 진행하는 경우 Score Scene으로 이동하며, 중단하고 Music Select Scene으로 되돌아갈 수 있다.
![인게임](https://github.com/pnucse-capstone/capstone-2023-1-10/assets/60247136/ffdd2e34-6f3d-48c6-a043-bfcd041ce65f)

### Score Scene

In Game Scene에서 진입하는 화면으로, In Game Scene에서 플레이한 결과를 출력한다.
![결과](https://github.com/pnucse-capstone/capstone-2023-1-10/assets/60247136/155ecbb1-6c15-4405-9380-ee957551fe2c)

## 4. 소개 및 시연 영상

### 게임 플레이 방법

![kr1](https://github.com/pnucse-capstone/capstone-2023-1-10/assets/60247136/35edc82d-cb8f-4e81-970a-9a9057187707)
![k2](https://github.com/pnucse-capstone/capstone-2023-1-10/assets/60247136/2fe5184b-036b-4631-9a60-eaacdd4eb1e8)
![kr3](https://github.com/pnucse-capstone/capstone-2023-1-10/assets/60247136/2ce2156a-023d-4e86-86b7-ab2954ff7429)
![kr4](https://github.com/pnucse-capstone/capstone-2023-1-10/assets/60247136/23b4a073-f64b-4f40-bb1e-985903036317)

### 시연 영상

[![시연영상](http://img.youtube.com/vi/hn8CCl24OZI/0.jpg)](https://youtu.be/hn8CCl24OZI)

## 5. 사용법

1. [Steam 상점 페이지](https://store.steampowered.com/app/1735670/roteroteSquare/?l=koreana) 방문
2. Steam 계정 로그인
3. 설치 및 실행
