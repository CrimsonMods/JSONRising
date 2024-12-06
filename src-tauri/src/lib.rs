use std::fs;
use tauri_plugin_dialog;
use tauri_plugin_fs;

#[tauri::command]
async fn save_filter_config(path: String, content: String) -> Result<(), String> {
    fs::write(path, content).map_err(|e| e.to_string())
}

#[tauri::command]
async fn load_filter_config(path: String) -> Result<String, String> {
    fs::read_to_string(path).map_err(|e| e.to_string())
}

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
        .plugin(tauri_plugin_fs::init())
        .plugin(tauri_plugin_dialog::init())
        .invoke_handler(tauri::generate_handler![
            save_filter_config,
            load_filter_config
        ])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}