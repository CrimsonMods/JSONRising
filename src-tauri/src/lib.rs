use serde_json::Value;
use std::fs;
use std::io::Read;
use tauri::path::BaseDirectory;
use tauri::Manager;
use tauri_plugin_dialog;
use tauri_plugin_fs;
use tauri_plugin_shell;

#[tauri::command]
async fn save_filter_config(path: String, content: String) -> Result<(), String> {
    fs::write(path, content).map_err(|e| e.to_string())
}

#[tauri::command]
async fn load_filter_config(path: String) -> Result<String, String> {
    fs::read_to_string(path).map_err(|e| e.to_string())
}

#[tauri::command]
fn get_app_version() -> String {
    env!("CARGO_PKG_VERSION").to_string()
}

#[tauri::command]
async fn check_for_updates() -> Result<Value, String> {
    let res = ureq::get("https://thunderstore.io/api/v1/package-metrics/skytech6/JSONRising/")
        .set("accept", "application/json")
        .set(
            "X-CSRFToken",
            "zcqFG6YabH2zDavW2AZAolERGOwAvAdWMlDAogfMvHQG6hdVwXoakWAzZsNHss8I",
        )
        .call()
        .map_err(|e| e.to_string())?;

    let mut body = String::new();
    res.into_reader()
        .read_to_string(&mut body)
        .map_err(|e| e.to_string())?;

    let json: Value = serde_json::from_str(&body).map_err(|e| e.to_string())?;

    Ok(json)
}

#[tauri::command]
async fn list_schema_files(app: tauri::AppHandle) -> Result<Vec<String>, String> {
    let schemas_dir = if cfg!(debug_assertions) {
        std::env::current_dir()
            .map_err(|e| e.to_string())?
            .join("resources/schemas")
    } else {
        app.path()
            .resolve("schemas", BaseDirectory::Resource)
            .expect("Should work")
    };

    let entries = fs::read_dir(&schemas_dir).map_err(|e| e.to_string())?;

    let schema_files: Vec<String> = entries
        .filter_map(|entry| {
            let entry = entry.ok()?;
            let path = entry.path();
            println!("Found file: {:?}", path);
            if path.extension()?.to_str()? == "json" {
                Some(path.file_name()?.to_str()?.to_owned())
            } else {
                None
            }
        })
        .collect();

    Ok(schema_files)
}

#[tauri::command]
async fn load_schema_file(path: String, app: tauri::AppHandle) -> Result<String, String> {
    let schemas_dir = if cfg!(debug_assertions) {
        let project_root = std::env::current_dir().expect("Should work");
        project_root.join("resources/schemas").join(&path)
    } else {
        app.path()
            .resolve(&path, BaseDirectory::Resource)
            .expect("Should work")
    };

    fs::read_to_string(schemas_dir).map_err(|e| e.to_string())
}

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
        .plugin(tauri_plugin_shell::init())
        .plugin(tauri_plugin_fs::init())
        .plugin(tauri_plugin_dialog::init())
        .setup(|_| Ok(()))
        .on_page_load(|window, _| {
            window
                .eval(
                    "window.addEventListener('click', (e) => {
                const target = e.target.closest('a');
                if (target && target.href && target.href.startsWith('http')) {
                    e.preventDefault();
                    e.stopPropagation();
                    window.__TAURI__.shell.open(target.href);
                }
            });",
                )
                .unwrap();
        })
        .invoke_handler(tauri::generate_handler![
            save_filter_config,
            load_filter_config,
            list_schema_files,
            load_schema_file,
            check_for_updates,
            get_app_version
        ])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
