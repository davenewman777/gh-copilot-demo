<template>
  <div class="app">
    <header class="header">
      <div class="language-control">
        <label for="language-select">{{ copy.languageLabel }}</label>
        <select id="language-select" v-model="selectedLanguage">
          <option
            v-for="language in languageOptions"
            :key="language.code"
            :value="language.code"
          >
            {{ language.label }}
          </option>
        </select>
      </div>
      <h1>🎵 {{ copy.appTitle }}</h1>
      <p>{{ copy.appSubtitle }}</p>
    </header>

    <main class="main">
      <div v-if="loading" class="loading">
        <div class="spinner"></div>
        <p>{{ copy.loadingAlbums }}</p>
      </div>

      <div v-else-if="hasError" class="error">
        <p>{{ copy.loadAlbumsError }}</p>
        <button @click="fetchAlbums" class="retry-btn">{{ copy.tryAgain }}</button>
      </div>

      <div v-else class="albums-grid">
        <AlbumCard 
          v-for="album in albums" 
          :key="album.id" 
          :album="album" 
          :add-to-cart-label="copy.addToCart"
          :preview-label="copy.preview"
        />
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import axios from 'axios'
import AlbumCard from './components/AlbumCard.vue'
import type { Album } from './types/album'
import { languageOptions, translations, type LanguageCode } from './translations'

// Fetch albums from the API
const albums = ref<Album[]>([])
// Set up loading and error states
const loading = ref<boolean>(true)
const hasError = ref<boolean>(false)
const selectedLanguage = ref<LanguageCode>('en')
const copy = computed(() => translations[selectedLanguage.value])

// Fetch albums when the component is mounted
const fetchAlbums = async (): Promise<void> => {
  try {
    loading.value = true
    hasError.value = false
    // Fetch albums from the API
    const response = await axios.get<Album[]>('/albums')
    albums.value = response.data
  } catch (err) {
    hasError.value = true
    console.error('Error fetching albums:', err)
  } finally {
    loading.value = false
  }
}

// Fetch albums when the component is mounted
onMounted(() => {
  fetchAlbums()
})
</script>

<style scoped>
.app {
  min-height: 100vh;
  padding: 2rem;
}

.header {
  position: relative;
  text-align: center;
  margin-bottom: 3rem;
  color: white;
}

.language-control {
  position: absolute;
  top: 0;
  right: 0;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.language-control label {
  font-size: 0.95rem;
  font-weight: 600;
}

.language-control select {
  min-width: 130px;
  padding: 0.5rem 0.75rem;
  color: #333;
  background: rgba(255, 255, 255, 0.95);
  border: 2px solid rgba(255, 255, 255, 0.7);
  border-radius: 8px;
  font-size: 0.95rem;
  cursor: pointer;
}

.header h1 {
  font-size: 3rem;
  margin-bottom: 0.5rem;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.3);
}

.header p {
  font-size: 1.2rem;
  opacity: 0.9;
}

.main {
  max-width: 1200px;
  margin: 0 auto;
}

.loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem;
  color: white;
}

.spinner {
  width: 50px;
  height: 50px;
  border: 4px solid rgba(255, 255, 255, 0.3);
  border-top: 4px solid white;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error {
  text-align: center;
  padding: 4rem;
  color: white;
}

.error p {
  font-size: 1.2rem;
  margin-bottom: 2rem;
}

.retry-btn {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: 2px solid white;
  padding: 0.75rem 2rem;
  border-radius: 25px;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.retry-btn:hover {
  background: white;
  color: #667eea;
}

.albums-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 2rem;
  padding: 1rem;
}

@media (max-width: 768px) {
  .app {
    padding: 1rem;
  }

  .language-control {
    position: static;
    justify-content: center;
    margin-bottom: 1.5rem;
  }
  
  .header h1 {
    font-size: 2rem;
  }
  
  .albums-grid {
    grid-template-columns: 1fr;
    gap: 1rem;
  }
}
</style>
