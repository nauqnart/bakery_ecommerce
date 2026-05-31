<template>
  <div class="notfound-page">
    <div class="notfound-card">
      <h1 class="error-code">404</h1>
      <h2 class="error-msg">Ối! Trang này không tồn tại</h2>
      <p class="error-desc">Có vẻ như bạn đã đi lạc hoặc đường dẫn không hợp lệ.</p>
      <router-link to="/" class="btn-home">Về trang chủ</router-link>
    </div>
  </div>
</template>

<style scoped>
.notfound-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #faf8f5;
  font-family: 'Inter', sans-serif;
  padding: 2rem;
}
.notfound-card {
  text-align: center;
  background: #fff;
  padding: 4rem;
  border-radius: 20px;
  box-shadow: 0 4px 24px rgba(0,0,0,0.04);
  border: 1px solid #e8e0d8;
  max-width: 500px;
  width: 100%;
}
.error-code {
  font-size: 6rem;
  font-weight: 800;
  color: #b36a3a;
  margin: 0 0 1rem;
  line-height: 1;
}
.error-msg {
  font-size: 1.5rem;
  font-weight: 700;
  color: #2c2018;
  margin: 0 0 0.5rem;
}
.error-desc {
  color: #8a7060;
  margin-bottom: 2rem;
}
.btn-home {
  display: inline-block;
  padding: 0.8rem 2rem;
  background: #b36a3a;
  color: #fff;
  text-decoration: none;
  border-radius: 10px;
  font-weight: 600;
  transition: all 0.2s;
}
.btn-home:hover {
  background: #9c592d;
  transform: translateY(-2px);
}
</style>
